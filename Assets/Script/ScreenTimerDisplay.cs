using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro

public class SceneTimerDisplay : MonoBehaviour
{
    [Header("Configuração do Timer")]
    public float timerDuration = 300f; // 5 minutos
    public string nextSceneName = "NextScene"; // Nome da cena que vai carregar

    [Header("UI")]
    public TMP_Text timerText; // Arraste seu TextMeshPro UI aqui

    private float timer;

    void Start()
    {
        timer = timerDuration;

        if (timerText == null)
        {
            Debug.LogError("TimerText não está atribuído! Arraste o TMP_Text no Inspector.");
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
