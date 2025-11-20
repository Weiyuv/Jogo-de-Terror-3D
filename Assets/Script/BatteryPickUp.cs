using UnityEngine;

public class BatteryPickUp : MonoBehaviour
{
    public float batteryAmount = 30f; // quanto recarrega
    public GameObject pickUpText;     // opcional: "Pressione E para pegar"

    private bool inReach = false;

    void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            // Encontra a lanterna e adiciona bateria
            Flashlight flashlight = Object.FindFirstObjectByType<Flashlight>();

            if (flashlight != null)
                flashlight.AddBattery(batteryAmount);

            // Desativa o objeto no mundo
            gameObject.SetActive(false);

            // Desativa texto
            if (pickUpText != null)
                pickUpText.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = true;
            if (pickUpText != null)
                pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inReach = false;
            if (pickUpText != null)
                pickUpText.SetActive(false);
        }
    }
}
