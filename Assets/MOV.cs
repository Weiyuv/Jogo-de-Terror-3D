using UnityEngine;

public class MOV : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private float velocidade = 4f;
    private Transform myCamera;
    private bool estaNoChao;
    [SerializeField] private Transform veficadorChao;
    [SerializeField] private LayerMask cenarioMask;
    [SerializeField] private float alturaDoSalto = 1f;
    private float gravidade = -9.81f;
    private float velocidadeVertical; 
    private void Awake()



    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;


    }


    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, myCamera.eulerAngles.y, transform.eulerAngles.z);  
        
        entradasJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        entradasJogador = transform.TransformDirection(entradasJogador); 

        characterController.Move(entradasJogador * Time.deltaTime * velocidade);
        estaNoChao = Physics.CheckSphere(veficadorChao.position, 0.3f, cenarioMask);
        if(Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoSalto * -2f * gravidade);
        }
        if(estaNoChao && velocidadeVertical < 0)
        {
            velocidadeVertical = -1f;
        }
        velocidadeVertical += gravidade * Time.deltaTime;

        characterController.Move(new Vector3(0, velocidadeVertical, 0) * Time.deltaTime); 
    }


}
