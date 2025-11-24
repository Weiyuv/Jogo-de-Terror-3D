using UnityEngine;
using UnityEngine.UI;

public class BotaoSair : MonoBehaviour
{
    [Header("Botão")]
    public Button meuBotao; // Arraste o botão aqui no Inspector

    void Start()
    {
        if (meuBotao != null)
        {
            meuBotao.onClick.AddListener(SairDoJogo);
        }
        else
        {
            Debug.LogWarning("Botão não está atribuído no Inspector!");
        }
    }

    void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

#if UNITY_EDITOR
        // Fecha o play mode no editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Fecha o jogo compilado
            Application.Quit();
#endif
    }
}
