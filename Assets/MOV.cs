using UnityEngine;

public class MOV : MonoBehaviour
{
    private Vector3 entradasJogador;
    private CharacterController characterController;
    private float velocidade = 4f;
    private Transform myCamera;
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
    }
}
