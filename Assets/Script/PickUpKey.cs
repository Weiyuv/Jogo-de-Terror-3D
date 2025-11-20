using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    public GameObject keyOB;      // Objeto físico da chave
    public GameObject pickUpText; // Texto "Pressione E para pegar"

    private bool inReach = false;

    void Start()
    {
        if (keyOB != null) keyOB.SetActive(true);
        if (pickUpText != null) pickUpText.SetActive(false);
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            if (keyOB != null) keyOB.SetActive(false);
            if (pickUpText != null) pickUpText.SetActive(false);

            // Marca que o jogador possui a chave
            PlayerInventory inv = FindObjectOfType<PlayerInventory>();
            if (inv != null)
                inv.hasKey = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = true;
            if (pickUpText != null) pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = false;
            if (pickUpText != null) pickUpText.SetActive(false);
        }
    }
}
