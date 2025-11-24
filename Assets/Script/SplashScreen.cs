using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float displayTime = 3f; // tempo em segundos
    public string nextScene = "MainScene"; // nome da cena do jogo

    void Start()
    {
        Invoke("LoadNextScene", displayTime);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
