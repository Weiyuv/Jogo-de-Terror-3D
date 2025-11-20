using UnityEngine;
using UnityEngine.SceneManagement; // se quiser mudar de cena

public class Door : MonoBehaviour
{
    public string requiredKeyName = "Key"; // não usado neste exemplo, mas pode ser útil
    private bool inReach = false;

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inv = FindObjectOfType<PlayerInventory>();
            if (inv != null && inv.hasKey)
            {
                // Aqui finaliza o jogo
                Debug.Log("Você abriu a porta e venceu o jogo!");
                // Se quiser fechar o jogo no build:
                Application.Quit();

                // Se quiser mudar de cena em vez de fechar:
                // SceneManager.LoadScene("CenaVitoria");
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
            inReach = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inReach = false;
    }
}
