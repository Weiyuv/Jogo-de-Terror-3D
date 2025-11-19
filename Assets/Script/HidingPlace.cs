using UnityEngine;
using UnityEngine.UI;

public class HidingPlace : MonoBehaviour
{
    [Header("UI")]
    public GameObject hideText;
    public GameObject exitText;

    [Header("Player")]
    public GameObject player;
    public Transform cameraTransform;

    [Header("Esconderijo")]
    public Transform hideCameraTransform;
    public Transform hidePlayerTransform;
    public float cameraMoveSpeed = 5f;

    private Renderer[] playerRenderers;

    private bool interactable = false;
    private bool hiding = false;
    private Vector3 originalCameraLocalPos;
    private Vector3 originalPlayerPos;

    // 👇 ADICIONADO — script de movimento
    private MOV3 move;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player não atribuído!");
            return;
        }

        playerRenderers = player.GetComponentsInChildren<Renderer>();

        // 👇 PEGA O SEU SCRIPT DE MOVIMENTO
        move = player.GetComponent<MOV3>();

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

        if (cameraTransform != null)
        {
            Vector3 targetPos = hiding ? hideCameraTransform.localPosition : originalCameraLocalPos;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * cameraMoveSpeed);
        }

        if (hiding && hidePlayerTransform != null)
        {
            player.transform.position = hidePlayerTransform.position;
        }
    }

    private void SetHiding(bool hide)
    {
        foreach (Renderer r in playerRenderers)
            r.enabled = !hide;

        // 👇 ATIVA/DESATIVA MOVIMENTO DO PLAYER
        if (move != null)
            move.enabled = !hide;

        if (hide)
        {
            if (hideText != null) hideText.SetActive(false);
            if (exitText != null) exitText.SetActive(true);
        }
        else
        {
            if (hideText != null) hideText.SetActive(true);
            if (exitText != null) exitText.SetActive(false);

            player.transform.position = originalPlayerPos;
        }
    }
}
