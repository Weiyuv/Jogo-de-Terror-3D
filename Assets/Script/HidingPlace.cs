using UnityEngine;
using UnityEngine.UI;

public class HidingPlace : MonoBehaviour
{
    [Header("UI")]
    public GameObject hideText; // texto "Press E to hide"
    public GameObject exitText; // texto "Press E to exit"

    [Header("Player")]
    public GameObject player;
    public Transform cameraTransform; // câmera do player

    [Header("Esconderijo")]
    public Transform hideCameraTransform; // posição da câmera dentro deste esconderijo
    public Transform hidePlayerTransform; // posição do player dentro do esconderijo
    public float cameraMoveSpeed = 5f;

    private Renderer[] playerRenderers;

    private bool interactable = false;
    private bool hiding = false;
    private Vector3 originalCameraLocalPos;
    private Vector3 originalPlayerPos;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player não atribuído!");
            return;
        }

        playerRenderers = player.GetComponentsInChildren<Renderer>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            originalCameraLocalPos = cameraTransform.localPosition;

        originalPlayerPos = player.transform.position;

        if (hideText != null)
            hideText.SetActive(false);
        if (exitText != null)
            exitText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;

            if (!hiding && hideText != null)
                hideText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;

            if (hideText != null)
                hideText.SetActive(false);
            if (exitText != null)
                exitText.SetActive(false);

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

        // Move a câmera suavemente para o Empty do esconderijo ou posição original
        if (cameraTransform != null)
        {
            Vector3 targetPos = hiding ? hideCameraTransform.localPosition : originalCameraLocalPos;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * cameraMoveSpeed);
        }

        // Teleporta player constantemente para o Empty se estiver escondido
        if (hiding && hidePlayerTransform != null)
        {
            player.transform.position = hidePlayerTransform.position;
        }
    }

    private void SetHiding(bool hide)
    {
        // Alterna visibilidade do player
        foreach (Renderer r in playerRenderers)
            r.enabled = !hide;

        // Exibe o texto correto
        if (hide)
        {
            if (hideText != null) hideText.SetActive(false);
            if (exitText != null) exitText.SetActive(true);
        }
        else
        {
            if (hideText != null) hideText.SetActive(true);
            if (exitText != null) exitText.SetActive(false);

            // Volta player para posição original
            player.transform.position = originalPlayerPos;
        }
    }
}
