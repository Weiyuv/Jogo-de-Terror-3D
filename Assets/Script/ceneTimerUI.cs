using UnityEngine;
using UnityEngine.UI;      // Para Text
using UnityEngine.SceneManagement;

public class SceneTimerUI : MonoBehaviour
{
    [Header("Configuração do Timer")]
    public float timerDuration = 300f; // 5 minutos em segundos
    public string nextSceneName = "NextScene"; // Nome da cena que vai carregar

    [Header("UI")]
    public Text timerText; // Arraste aqui o Text do Canvas

    private float timer;

    void Start()
    {
        // Inicializa o timer
        timer = timerDuration;

        if (timerText == null)
        {
            Debug.LogError("TimerText não está atribuído!");
        }
        else
        {
            UpdateTimerUI();
        }
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer < 0f) timer = 0f;

            UpdateTimerUI();
        }
        else
        {
            // Troca de cena quando o timer zerar
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
