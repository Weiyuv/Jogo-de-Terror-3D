using UnityEngine;

public class som : MonoBehaviour
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
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Som detectável pelo inimigo")]
    public BlindEnemy[] blindEnemies;

    [Header("Configuração de som")]
    [Range(0f, 5f)]
    public float soundRangeMultiplier = 1f;

    [Header("Player")]
    public MOV3 playerMov; // referência ao MOV3

    void Update()
    {
        if (blindEnemies == null || blindEnemies.Length == 0)
            blindEnemies = Object.FindObjectsByType<BlindEnemy>(FindObjectsSortMode.None);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        bool moving =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        bool running = Input.GetKey(runKey);
        bool crouching = Input.GetKey(crouchKey);

        if (moving && isGrounded)
        {
            if (running && playerMov != null && playerMov.stamina > 0)
            {
                PlayOnly(footstepsRun);
                AlertEnemies(3f * soundRangeMultiplier);
            }
            else if (running && (playerMov == null || playerMov.stamina <= 0))
            {
                PlayOnly(footstepsNormal);
                AlertEnemies(0.5f * soundRangeMultiplier);
            }
            else if (crouching)
            {
                PlayOnly(footstepsCrouch);
                AlertEnemies(0f);
            }
            else
            {
                PlayOnly(footstepsNormal);
                AlertEnemies(0.5f * soundRangeMultiplier);
            }
        }
        else
        {
            StopAll();
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            if (jumpSound != null)
                jumpSound.PlayOneShot(jumpSound.clip);

            AlertEnemies(0f * soundRangeMultiplier);
        }
    }

    void PlayOnly(AudioSource source)
    {
        if (source == null) return;

        if (!source.isPlaying)
            source.Play();

        if (source != footstepsNormal && footstepsNormal.isPlaying)
            footstepsNormal.Stop();

        if (source != footstepsRun && footstepsRun.isPlaying)
            footstepsRun.Stop();

        if (source != footstepsCrouch && footstepsCrouch.isPlaying)
            footstepsCrouch.Stop();
    }

    void StopAll()
    {
        if (footstepsNormal != null) footstepsNormal.Stop();
        if (footstepsRun != null) footstepsRun.Stop();
        if (footstepsCrouch != null) footstepsCrouch.Stop();
    }

    void AlertEnemies(float volumeMultiplier = 1f)
    {
        foreach (BlindEnemy enemy in blindEnemies)
        {
            if (enemy != null)
                enemy.HearSound(transform.position, volumeMultiplier);
        }
    }
}
