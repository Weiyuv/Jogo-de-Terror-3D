using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("UI")]
    public GameObject doorText;      // Texto "Pressione E para abrir"
    public GameObject needKeyText;   // Texto "Você precisa da chave!"

    [Header("Cena")]
    public string sceneToLoad = "CenaVitoria";

    private bool inReach = false;
    private float keyTextTimer = 0f;
    private float keyTextDuration = 2f; // tempo que a mensagem fica na tela

    void Start()
    {
        if (doorText != null)
            doorText.SetActive(false);

        if (needKeyText != null)
            needKeyText.SetActive(false);
    }

    void Update()
    {
        // SE O JOGADOR ESTIVER PERTO E APERTAR E
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inv = FindFirstObjectByType<PlayerInventory>();

            if (inv != null && inv.hasKey)
            {
                // Abre a porta e troca de cena
                if (doorText != null)
                    doorText.SetActive(false);

                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                // MOSTRA TEXTO DE "PRECISA DA CHAVE"
                if (needKeyText != null)
                {
                    needKeyText.SetActive(true);
                    keyTextTimer = keyTextDuration;
                }
            }
        }

        // Contagem para esconder texto "precisa da chave"
        if (needKeyText != null && needKeyText.activeSelf)
        {
            keyTextTimer -= Time.deltaTime;
            if (keyTextTimer <= 0)
            {
                needKeyText.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = true;

            if (doorText != null)
                doorText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = false;

            if (doorText != null)
                doorText.SetActive(false);

            if (needKeyText != null)
                needKeyText.SetActive(false);
        }
    }
}
