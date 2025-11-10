using UnityEngine;
using UnityEngine.UI;

public class MOV : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private Transform myCamera;
    private bool estaNoChao;

    [Header("Movimento")]
    public float velocidadeNormal = 4f;
    public float velocidadeAgachar = 2f; // controle via Inspector
    public float velocidadeCorrida = 7f;  // controle via Inspector

    [Header("Pulo")]
    public float alturaDoSalto = 1.5f;
    private float gravidade = -60f;
    private float velocidadeVertical;

    [Header("Agachar")]
    private float alturaOriginal;
    public float alturaAgachar = 1f;
    private Vector3 centroOriginal;
    private Vector3 centroAgachar;
    public float suavizacaoAgachar = 5f; // controla a velocidade da transição

    [Header("Stamina")]
    public float staminaMax = 100f;
    private float stamina;
    public float gastoCorrida = 20f;       // por segundo
    public float recuperacaoStamina = 15f; // por segundo
    public Slider sliderStamina;           // arraste o slider aqui

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;

        alturaOriginal = characterController.height;
        centroOriginal = characterController.center;
        centroAgachar = new Vector3(centroOriginal.x, alturaAgachar / 2, centroOriginal.z);

        stamina = staminaMax;
    }

    void Update()
    {
        // Rotação de acordo com a câmera
        transform.eulerAngles = new Vector3(0, myCamera.eulerAngles.y, 0);

        // Movimento no plano (WASD)
        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradasJogador = transform.TransformDirection(entradasJogador);

        // Verificação do chão
        estaNoChao = Physics.CheckSphere(characterController.transform.position + Vector3.down * (characterController.height / 2), 0.3f, LayerMask.GetMask("Default"));

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
        }

        // Gravidade
        if (estaNoChao && velocidadeVertical < 0)
        {
            velocidadeVertical = -2f;
        }
        velocidadeVertical += gravidade * Time.deltaTime;
        characterController.Move(Vector3.up * velocidadeVertical * Time.deltaTime);

        // Agachar
        bool agachando = Input.GetKey(KeyCode.LeftControl);

        // Alvo para height e center
        float targetHeight = agachando ? alturaAgachar : alturaOriginal;
        Vector3 targetCenter = agachando ? centroAgachar : centroOriginal;

        // Lerp para transição suave
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
            // velocidade normal
            velocidadeAtual = velocidadeNormal;
            if (stamina < staminaMax)
            {
                stamina += recuperacaoStamina * Time.deltaTime;
                if (stamina > staminaMax) stamina = staminaMax;
            }
        }

        characterController.Move(entradasJogador * Time.deltaTime * velocidadeAtual);

        // Atualizar HUD da stamina
        if (sliderStamina != null)
        {
            sliderStamina.value = stamina / staminaMax;
        }
    }
}
