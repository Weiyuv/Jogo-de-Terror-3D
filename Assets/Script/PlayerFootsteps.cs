using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("AudioSources (arraste 4)")]
    public AudioSource footstepsNormal;
    public AudioSource footstepsRun;
    public AudioSource footstepsCrouch;
    public AudioSource jumpSound;

    [Header("Teclas")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Detecção de chão")]
    public Transform groundCheck;       // um empty sob os pés
    public float groundDistance = 0.3f; // raio da checagem
    public LayerMask groundMask;        // layer do chão
    private bool isGrounded;

    [Header("Som detectável pelo inimigo")]
    public BlindEnemy[] blindEnemies;   // todos os inimigos cegos na cena

    void Update()
    {
        // --- Checa se está no chão ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Movimento
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                    || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        bool running = Input.GetKey(runKey);
        bool crouching = Input.GetKey(crouchKey);

        // --- Lógica de passos ---
        if (moving && isGrounded)
        {
            if (running)
            {
                PlayOnly(footstepsRun);
                AlertEnemies(); // emite som forte
            }
            else if (crouching)
            {
                PlayOnly(footstepsCrouch);
                AlertEnemies(0.5f); // som mais fraco
            }
            else
            {
                PlayOnly(footstepsNormal);
                AlertEnemies(); // som normal
            }
        }
        else
        {
            StopAll();
        }

        // --- Som de pulo ---
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            if (jumpSound != null)
                jumpSound.PlayOneShot(jumpSound.clip);

            AlertEnemies(1.2f); // som mais alto pro pulo
        }
    }

    // 🔊 Toca só o som escolhido e para os outros
    void PlayOnly(AudioSource source)
    {
        if (source == null) return;

        if (!source.isPlaying)
            source.Play();

        // parar os outros
        if (source != footstepsNormal && footstepsNormal.isPlaying)
            footstepsNormal.Stop();
        if (source != footstepsRun && footstepsRun.isPlaying)
            footstepsRun.Stop();
        if (source != footstepsCrouch && footstepsCrouch.isPlaying)
            footstepsCrouch.Stop();
    }

    // 🔇 Para todos
    void StopAll()
    {
        if (footstepsNormal.isPlaying) footstepsNormal.Stop();
        if (footstepsRun.isPlaying) footstepsRun.Stop();
        if (footstepsCrouch.isPlaying) footstepsCrouch.Stop();
    }

    // 👂 Informa os inimigos cegos que o jogador fez barulho
    void AlertEnemies(float volumeMultiplier = 1f)
    {
        if (blindEnemies == null || blindEnemies.Length == 0) return;

        foreach (BlindEnemy enemy in blindEnemies)
        {
            if (enemy != null)
                enemy.HearSound(transform.position, volumeMultiplier);
        }
    }
}
