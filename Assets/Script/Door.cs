using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public GameObject doorText; // Texto tipo "Pressione E para abrir"
    private bool inReach = false;

    void Start()
    {
        if (doorText != null)
            doorText.SetActive(false); // Começa invisível
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inv = FindObjectOfType<PlayerInventory>();
            if (inv != null && inv.hasKey)
            {
                // Aqui você abre a porta / finaliza o jogo
                Debug.Log("Você abriu a porta e venceu o jogo!");
                if (doorText != null)
                    doorText.SetActive(false); // Esconde texto após abrir

                Application.Quit(); // ou SceneManager.LoadScene("CenaVitoria");
            }
            else
            {
                Debug.Log("Você precisa da chave para abrir a porta!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = true;
            if (doorText != null)
                doorText.SetActive(true); // Mostra texto ao entrar
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = false;
            if (doorText != null)
                doorText.SetActive(false); // Esconde texto ao sair
        }
    }
}
