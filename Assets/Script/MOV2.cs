using UnityEngine;

public class MOV2 : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private float velocidade = 4f;
    private Transform myCamera;
    private bool estaNoChao;
    [SerializeField] private Transform veficadorChao;
    [SerializeField] private LayerMask cenarioMask;

    [Header("Pulo")]
    [SerializeField] private float alturaDoSalto = 1.5f;
    private float gravidade = -20f;
    private float velocidadeVertical;
    [SerializeField] private float custoPulo = 15f; // stamina gasta por pulo

    [Header("Stamina")]
    public float staminaMax = 100f;
    private float stamina;
    public float gastoCorrida = 20f;
    public float recuperacaoStamina = 15f;
    public float delayRecuperacao = 1f; // segundos antes de regenerar
    private float tempoDesdeUltimaAcao = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;
        stamina = staminaMax;
    }

    void Update()
    {
        // Rotação de acordo com a câmera
        transform.eulerAngles = new Vector3(0, myCamera.eulerAngles.y, 0);

        // Movimento no plano (WASD)
        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradasJogador = transform.TransformDirection(entradasJogador);
        characterController.Move(entradasJogador * Time.deltaTime * velocidade);

        // Verificação do chão
        estaNoChao = Physics.CheckSphere(veficadorChao.position, 0.3f, cenarioMask);

        // Pulo com custo de stamina
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao && stamina >= custoPulo)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
            stamina -= custoPulo;
            if (stamina < 0) stamina = 0;
            tempoDesdeUltimaAcao = 0f; // reseta delay
        }

        // Aplicar gravidade natural
        if (estaNoChao && velocidadeVertical < 0)
            velocidadeVertical = -2f;

        velocidadeVertical += gravidade * Time.deltaTime;
        characterController.Move(Vector3.up * velocidadeVertical * Time.deltaTime);

        // Corrida (mantendo simplicidade)
        bool correndo = Input.GetKey(KeyCode.LeftShift) && entradasJogador.magnitude > 0;
        if (correndo && stamina > 0)
        {
            velocidade = 7f; // velocidade corrida
            stamina -= gastoCorrida * Time.deltaTime;
            if (stamina < 0) stamina = 0;
            tempoDesdeUltimaAcao = 0f; // reseta delay
        }
        else
        {
            velocidade = 4f; // velocidade normal
            // delay para regenerar stamina
            tempoDesdeUltimaAcao += Time.deltaTime;
            if (tempoDesdeUltimaAcao >= delayRecuperacao && stamina < staminaMax)
            {
                stamina += recuperacaoStamina * Time.deltaTime;
                if (stamina > staminaMax) stamina = staminaMax;
            }
        }
    }
}
