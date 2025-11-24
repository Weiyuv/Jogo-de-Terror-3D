using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    [Header("UI")]
    public GameObject hideText;
    public GameObject exitText;

    [Header("Player")]
    public GameObject player;

    [Header("Câmeras")]
    public Camera mainCamera;
    public Camera hideCamera;

    [Header("Lanterna")]
    public Light spotLight;
    private float spotIntensity;

    private bool interactable = false;
    private bool hiding = false;

    private Vector3 savedPlayerPos;
    private Quaternion savedPlayerRot;

    private Renderer[] playerRenderers;

    private void Start()
    {
        if (spotLight != null)
            spotIntensity = spotLight.intensity;

        if (hideText) hideText.SetActive(false);
        if (exitText) exitText.SetActive(false);

        if (hideCamera != null)
            hideCamera.enabled = false;

        // pega todos os renderers do player
        playerRenderers = player.GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;
            if (!hiding && hideText)
                hideText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;
            if (hideText) hideText.SetActive(false);
            if (exitText) exitText.SetActive(false);

            if (hiding)
                SetHiding(false);
        }
    }

    private void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            hiding = !hiding;
            SetHiding(hiding);
        }
    }

    private void SetHiding(bool hide)
    {
        if (hide)
        {
            // salvar posição do player
            savedPlayerPos = player.transform.position;
            savedPlayerRot = player.transform.rotation;

            // desligar render do player (visual)
            foreach (var rend in playerRenderers)
                rend.enabled = false;

            // desligar a câmera principal
            if (mainCamera != null)
                mainCamera.enabled = false;

            // ligar a câmera do esconderijo
            if (hideCamera != null)
                hideCamera.enabled = true;

            // desligar lanterna
            if (spotLight != null)
                spotLight.intensity = 0f;

            if (hideText) hideText.SetActive(false);
            if (exitText) exitText.SetActive(true);
        }
        else
        {
            // reativar render do player
            foreach (var rend in playerRenderers)
                rend.enabled = true;

            // reposicionar player (opcional)
            player.transform.position = savedPlayerPos;
            player.transform.rotation = savedPlayerRot;

            // ligar a câmera do player
            if (mainCamera != null)
                mainCamera.enabled = true;

            // desligar câmera do esconderijo
            if (hideCamera != null)
                hideCamera.enabled = false;

            // restaura lanterna
            if (spotLight != null)
                spotLight.intensity = spotIntensity;

            if (hideText) hideText.SetActive(true);
            if (exitText) exitText.SetActive(false);
        }
    }
}
