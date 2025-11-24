using UnityEngine;
using UnityEngine.UI;

public class MOV3 : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private bool estaNoChao;
    private float velocidadeVertical;

    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeAgachar = 2f;
    public float velocidadeCorrida = 7f;

    [Header("Pulo")]
    public float alturaDoSalto = 1.5f;
    public float gravidade = -20f;
    public float gastoPulo = 15f;

    [Header("Cabeça / Câmera")]
    public Transform headCamera;
    public float alturaHead = 0.5f;
    public float suavizacaoAgachar = 5f;
    private Vector3 posCameraOriginal;
    private Vector3 posCameraAgachar;

    private Transform myCamera;

    [Header("Chão")]
    [SerializeField] private Transform veficadorChao;
    [SerializeField] private LayerMask cenarioMask;
    [SerializeField] private float raioChao = 0.3f;

    [Header("Stamina")]
    public float staminaMax = 100f;
    public float gastoCorrida = 20f;
    public float recuperacaoStamina = 15f;
    public float delayRecuperacao = 1f;
    private float timerRecuperacao = 0f;
    public Image staminaImage;
    public float tempoSumir = 2f;
    private float timerSumir = 0f;

    // ✅ Stamina pública para outros scripts
    public float stamina { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;

        if (headCamera == null && Camera.main != null)
            headCamera = Camera.main.transform;

        if (headCamera != null)
        {
            posCameraOriginal = headCamera.localPosition;
            posCameraAgachar = posCameraOriginal + new Vector3(0, -alturaHead, 0);
        }

        stamina = staminaMax;

        if (staminaImage != null)
            staminaImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (headCamera == null) return;

        transform.eulerAngles = new Vector3(0, myCamera.eulerAngles.y, 0);

        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradasJogador = transform.TransformDirection(entradasJogador);

        estaNoChao = Physics.CheckSphere(veficadorChao.position, raioChao, cenarioMask);

        bool gastandoStamina = false;

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && stamina >= gastoPulo)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
            stamina -= gastoPulo;
            timerRecuperacao = 0f;
            gastandoStamina = true;
        }

        velocidadeVertical += gravidade * Time.deltaTime;

        // Agachar / câmera
        bool agachando = Input.GetKey(KeyCode.LeftControl);
        Vector3 alvoCamera = agachando ? posCameraAgachar : posCameraOriginal;
        headCamera.localPosition = Vector3.Lerp(headCamera.localPosition, alvoCamera, Time.deltaTime * suavizacaoAgachar);

        // Corrida
        bool correndo = Input.GetKey(KeyCode.LeftShift) && entradasJogador.magnitude > 0 && !agachando && stamina > 0;

        float velocidadeAtual = velocidadeNormal;

        if (correndo)
        {
            velocidadeAtual = velocidadeCorrida;
            stamina -= gastoCorrida * Time.deltaTime;
            timerRecuperacao = 0f;
            gastandoStamina = true;
        }
        else
        {
            velocidadeAtual = agachando ? velocidadeAgachar : velocidadeNormal;

            if (timerRecuperacao >= delayRecuperacao)
                stamina += recuperacaoStamina * Time.deltaTime;
            else
                timerRecuperacao += Time.deltaTime;
        }

        // Limita stamina e desativa corrida se zerar
        stamina = Mathf.Clamp(stamina, 0, staminaMax);

        Vector3 movimento = entradasJogador * velocidadeAtual * Time.deltaTime;
        movimento.y = velocidadeVertical * Time.deltaTime;

        CollisionFlags flags = characterController.Move(movimento);
        if ((flags & CollisionFlags.Below) != 0 && velocidadeVertical < 0)
            velocidadeVertical = 0f;

        // HUD da Stamina
        if (staminaImage != null)
        {
            staminaImage.fillAmount = stamina / staminaMax;
            staminaImage.color = Color.Lerp(Color.red, Color.green, stamina / staminaMax);

            if (gastandoStamina)
            {
                staminaImage.gameObject.SetActive(true);
                timerSumir = 0f;
            }
            else
            {
                if (stamina >= staminaMax)
                {
                    timerSumir += Time.deltaTime;
                    if (timerSumir >= tempoSumir)
                        staminaImage.gameObject.SetActive(false);
                }
                else
                {
                    staminaImage.gameObject.SetActive(true);
                }
            }
        }
    }
}
