using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotaoMudarCena : MonoBehaviour
{
    [Header("Botão")]
    public Button meuBotao;  // Arraste o botão aqui no Inspector
    public string nomeCena = "LABLAB"; // Nome da cena para mudar

    void Start()
    {
        if (meuBotao != null)
        {
            meuBotao.onClick.AddListener(MudarCena);
        }
        else
        {
            Debug.LogWarning("Botão não está atribuído no Inspector!");
        }
    }

    void MudarCena()
    {
        SceneManager.LoadScene(nomeCena);
    }
}
