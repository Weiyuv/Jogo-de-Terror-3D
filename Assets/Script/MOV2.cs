using UnityEngine;
using UnityEngine.UI;

public class MOV2 : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private Transform myCamera;
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

    [Header("Agachar")]
    public float alturaAgachar = 1f;
    private float alturaOriginal;
    private Vector3 centroOriginal;
    private Vector3 centroAgachar;
    public float suavizacaoAgachar = 5f;

    [Header("Chão")]
    [SerializeField] private Transform veficadorChao;
    [SerializeField] private LayerMask cenarioMask;
    [SerializeField] private float raioChao = 0.3f;

    [Header("Stamina")]
    public float staminaMax = 100f;
    private float stamina;
    public float gastoCorrida = 20f;
    public float recuperacaoStamina = 15f;
    public float delayRecuperacao = 1f;
    private float timerRecuperacao = 0f;
    public Image staminaImage; // Image Filled
    public float tempoSumir = 2f; // tempo para barra sumir após não usar
    private float timerSumir = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;

        alturaOriginal = characterController.height;
        centroOriginal = characterController.center;
        centroAgachar = new Vector3(centroOriginal.x, alturaAgachar / 2f, centroOriginal.z);

        stamina = staminaMax;

        if (staminaImage != null)
            staminaImage.gameObject.SetActive(false); // começa escondida
    }

    void Update()
    {
        // Rotação da câmera
        transform.eulerAngles = new Vector3(0, myCamera.eulerAngles.y, 0);

        // Movimento horizontal
        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradasJogador = transform.TransformDirection(entradasJogador);

        // Verificação do chão
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

        // Aplicar gravidade
        velocidadeVertical += gravidade * Time.deltaTime;

        // Agachar
        bool agachando = Input.GetKey(KeyCode.LeftControl);
        float targetHeight = agachando ? alturaAgachar : alturaOriginal;
        Vector3 targetCenter = agachando ? centroAgachar : centroOriginal;

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * suavizacaoAgachar);
        characterController.center = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * suavizacaoAgachar);

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

            // Delay para recuperar stamina
            if (timerRecuperacao >= delayRecuperacao)
                stamina += recuperacaoStamina * Time.deltaTime;
            else
                timerRecuperacao += Time.deltaTime;
        }

        // Limitar stamina
        if (stamina > staminaMax) stamina = staminaMax;
        if (stamina < 0) stamina = 0;

        // Movimento final
        Vector3 movimento = entradasJogador * velocidadeAtual * Time.deltaTime;
        movimento.y = velocidadeVertical * Time.deltaTime;
        CollisionFlags flags = characterController.Move(movimento);

        if ((flags & CollisionFlags.Below) != 0 && velocidadeVertical < 0)
            velocidadeVertical = 0f;

        // Atualizar barra de stamina
        if (staminaImage != null)
        {
            staminaImage.fillAmount = stamina / staminaMax;
            staminaImage.color = Color.Lerp(Color.red, Color.green, stamina / staminaMax);

            if (gastandoStamina)
            {
                staminaImage.gameObject.SetActive(true);
                timerSumir = 0f; // reinicia timer para sumir
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
