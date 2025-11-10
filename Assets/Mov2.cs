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
    [SerializeField] private float alturaDoSalto = 1.5f;
    private float gravidade = -20f;
    private float velocidadeVertical;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;
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

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
        }

        // Aplicar gravidade mais natural
        if (estaNoChao && velocidadeVertical < 0)
        {
            velocidadeVertical = -2f; // “gruda” melhor no chão sem flutuar
        }

        // Queda com aceleração realista
        velocidadeVertical += gravidade * Time.deltaTime;
        characterController.Move(Vector3.up * velocidadeVertical * Time.deltaTime);
    }
}
