using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMov : MonoBehaviour
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
    private float xRotation = 0f;

    private CharacterController controller;
    private Keyboard kb;
    private Mouse mouse;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        controller.center = new Vector3(0, originalHeight / 2f, 0);

        kb = Keyboard.current;
        mouse = Mouse.current;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (kb == null || mouse == null) return;

        HandleMovement();
        HandleCamera();
        HandleJump();
        HandleCrouch();
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (kb.wKey.isPressed) moveZ += 1f;
        if (kb.sKey.isPressed) moveZ -= 1f;
        if (kb.dKey.isPressed) moveX += 1f;
        if (kb.aKey.isPressed) moveX -= 1f;

        // Corrida estilo CS:GO
        bool isRunning = kb.leftShiftKey.isPressed && !isCrouching;
        currentSpeed = isRunning ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.Normalize(); // evita mover mais rápido na diagonal
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleCamera()
    {
        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime * 100f;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime * 100f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        if (kb.spaceKey.wasPressedThisFrame && controller.isGrounded)
            verticalVelocity = jumpForce;

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (kb.leftCtrlKey.wasPressedThisFrame)
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // Ajusta o center para o collider ficar correto
        Vector3 c = controller.center;
        c.y = controller.height / 2f;
        controller.center = c;
    }
}
