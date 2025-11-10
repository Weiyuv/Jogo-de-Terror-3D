using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Mov2 : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2.5f;
    private float currentSpeed;

    [Header("Pulo e Gravidade")]
    public float jumpForce = 6f;
    public float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Agachar")]
    public float crouchHeight = 1f;
    private float originalHeight;
    public float crouchTransitionSpeed = 10f;
    private bool isCrouching = false;

    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;

    private CharacterController controller;

    // Armazena a rotação acumulada da câmera
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        controller.center = new Vector3(0, originalHeight / 2f, 0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleJump();
        HandleCrouch();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        currentSpeed = isRunning ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        // Move de acordo com a direção atual da câmera
        Vector3 move = playerCamera.forward * moveZ + playerCamera.right * moveX;
        move.y = 0; // mantém movimento no plano
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Acumula rotação (sem clamp)
        rotationY += mouseX;
        rotationX -= mouseY;

        // Aplica a rotação livre em todos os eixos
        playerCamera.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void HandleJump()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            verticalVelocity = jumpForce;

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        Vector3 c = controller.center;
        c.y = controller.height / 2f;
        controller.center = c;
    }
}
