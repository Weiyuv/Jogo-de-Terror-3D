using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configuração")]
    public float restartDelay = 2f;
    public string deathSceneName = "DeathScene";

    [Header("Câmeras")]
    public Camera playerCam;
    public Camera monsterCam;

    [Header("Som")]
    public AudioSource deathSound; // Arraste o som aqui no Inspector

    private GameObject player;
    private bool playerDied = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            Debug.LogError("Nenhum objeto com a tag 'Player' foi encontrado na cena!");

        if (monsterCam != null)
            monsterCam.enabled = false; // começa desligada
    }

    void Update()
    {
        if (!playerDied && player == null) // Player acabou de morrer
        {
            playerDied = true;

            Debug.Log("Player destruído! Iniciando jumpscare...");

            DoJumpScare();

            Invoke(nameof(LoadDeathScene), restartDelay);
        }
    }

    void DoJumpScare()
    {
        // 🔥 Troca de câmera
        if (playerCam != null) playerCam.enabled = false;
        if (monsterCam != null) monsterCam.enabled = true;

        // 🔥 Som de morte
        if (deathSound != null)
        {
            deathSound.Play();
        }
        else
        {
            Debug.LogWarning("⚠ Nenhum áudio de morte atribuído ao GameManager.");
        }

        // 🔥 Iluminação dramática
        RenderSettings.ambientLight = Color.red * 0.3f;
        RenderSettings.ambientIntensity = 0.2f;

        RenderSettings.fogColor = Color.red * 0.6f;
        RenderSettings.fogDensity = 0.1f;

        Debug.Log("Iluminação modificada para jumpscare!");
    }

    void LoadDeathScene()
    {
        SceneManager.LoadScene(deathSceneName);
    }
}
