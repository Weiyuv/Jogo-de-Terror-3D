using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Painel de Pausa")]
    public GameObject pausePanel; // Arraste seu painel de pausa aqui

    private bool isPaused = false;

    void Start()
    {
        // Garante que o painel começa escondido
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        // Ao pressionar Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);   // Mostra o painel
        Time.timeScale = 0f;          // Congela o jogo
        AudioListener.pause = true;   // Pausa todos os sons da cena
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(false);  // Esconde o painel
        Time.timeScale = 1f;          // Volta ao normal
        AudioListener.pause = false;  // Retoma todos os sons
        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();           // Sai do jogo
    }
}
