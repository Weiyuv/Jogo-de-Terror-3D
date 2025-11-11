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

    void Update()
    {
        // Movimento
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                    || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        bool running = Input.GetKey(runKey);
        bool crouching = Input.GetKey(crouchKey);

        // --- Lógica de passos ---
        if (moving)
        {
            if (running)
            {
                PlayOnly(footstepsRun);
            }
            else if (crouching)
            {
                PlayOnly(footstepsCrouch);
            }
            else
            {
                PlayOnly(footstepsNormal);
            }
        }
        else
        {
            StopAll();
        }

        // --- Som de pulo ---
        if (Input.GetKeyDown(jumpKey))
        {
            if (jumpSound != null)
                jumpSound.PlayOneShot(jumpSound.clip);
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
}
