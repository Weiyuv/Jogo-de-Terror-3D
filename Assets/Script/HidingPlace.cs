using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    [Header("UI")]
    public GameObject hideText; // texto "Press E to hide"

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
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hideText != null)
                hideText.SetActive(true);
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hideText != null)
                hideText.SetActive(false);
            interactable = false;

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

            if (hideText != null)
                hideText.SetActive(false);
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

        // Se saiu do esconderijo, volta player para posição original
        if (!hide)
            player.transform.position = originalPlayerPos;
    }
}
