using UnityEngine;
using UnityEngine.UI;

public class MOV2 : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private Transform myCamera;
    private bool estaNoChao;

    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeAgachar = 2f;
    public float velocidadeCorrida = 7f;

    [Header("Pulo")]
    public float alturaDoSalto = 1.5f;
    public float gravidade = -20f;
    private float velocidadeVertical;

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
    public Slider sliderStamina;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;

        alturaOriginal = characterController.height;
        centroOriginal = characterController.center;
        centroAgachar = new Vector3(centroOriginal.x, alturaAgachar / 2f, centroOriginal.z);

        stamina = staminaMax;
    }

    void Update()
    {
        // Rotação de acordo com a câmera
        transform.eulerAngles = new Vector3(0, myCamera.eulerAngles.y, 0);

        // Movimento horizontal
        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradasJogador = transform.TransformDirection(entradasJogador);

        // Verificação do chão usando veficadorChao
        estaNoChao = Physics.CheckSphere(veficadorChao.position, raioChao, cenarioMask);

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
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
        bool correndo = Input.GetKey(KeyCode.LeftShift) && entradasJogador.magnitude > 0 && !agachando;
        float velocidadeAtual = velocidadeNormal;

        if (correndo && stamina > 0)
        {
            velocidadeAtual = velocidadeCorrida;
            stamina -= gastoCorrida * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
        else if (agachando)
        {
            velocidadeAtual = velocidadeAgachar;
            if (stamina < staminaMax)
            {
                stamina += recuperacaoStamina * Time.deltaTime;
                if (stamina > staminaMax) stamina = staminaMax;
            }
        }
        else
        {
            velocidadeAtual = velocidadeNormal;
            if (stamina < staminaMax)
            {
                stamina += recuperacaoStamina * Time.deltaTime;
                if (stamina > staminaMax) stamina = staminaMax;
            }
        }

        // Movimento final
        Vector3 movimento = entradasJogador * velocidadeAtual * Time.deltaTime;
        movimento.y = velocidadeVertical * Time.deltaTime;
        CollisionFlags flags = characterController.Move(movimento);

        // Resetar velocidade vertical ao tocar o chão
        if ((flags & CollisionFlags.Below) != 0 && velocidadeVertical < 0)
        {
            velocidadeVertical = 0f;
        }

        // Atualizar HUD da stamina
        if (sliderStamina != null)
        {
            sliderStamina.value = stamina / staminaMax;
        }
    }
}
