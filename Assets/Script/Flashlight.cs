using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    [Header("Configuração da luz")]
    public Light spotLight;            // sua Spot Light
    public bool startOn = false;       // começa ligada?
    public float scareDistance = 10f;  // alcance que assusta
    public float scareAngle = 30f;     // ângulo do cone
    public float maxBattery = 100f;    // bateria máxima
    public float batteryConsumption = 10f; // consumo por segundo

    [Header("UI")]
    public Image batteryFill;          // Image com tipo Filled
    public float uiDisplayTime = 2f;   // tempo que a barra fica visível após recarga

    [HideInInspector] public float currentBattery;

    private Coroutine hideUICoroutine;

    void Start()
    {
        currentBattery = maxBattery;
        if (spotLight != null)
            spotLight.enabled = startOn;

        UpdateBatteryUI();
        UpdateBatteryVisibility();
    }

    void Update()
    {
        if (spotLight == null) return;

        // Liga/desliga com F
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!spotLight.enabled && currentBattery > 0f)
                spotLight.enabled = true;
            else
                spotLight.enabled = false;

            UpdateBatteryVisibility();
        }

        // Consumo de bateria
        if (spotLight.enabled)
        {
            currentBattery -= batteryConsumption * Time.deltaTime;
            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                spotLight.enabled = false; // desliga automaticamente
            }

            ScareEnemies();
            UpdateBatteryUI();
        }
    }

    // Método para recarregar bateria quando pegar item no mapa
    public void AddBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;

        UpdateBatteryUI();
        ShowBatteryUI();
    }

    void UpdateBatteryUI()
    {
        if (batteryFill != null)
            batteryFill.fillAmount = currentBattery / maxBattery;
    }

    void UpdateBatteryVisibility()
    {
        if (batteryFill != null)
            batteryFill.gameObject.SetActive(spotLight.enabled);
    }

    void ShowBatteryUI()
    {
        if (batteryFill == null) return;

        batteryFill.gameObject.SetActive(true);

        // Cancela corrotina anterior
        if (hideUICoroutine != null)
            StopCoroutine(hideUICoroutine);

        // Inicia corrotina para esconder a UI após um tempo
        hideUICoroutine = StartCoroutine(HideUIAfterDelay());
    }

    IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(uiDisplayTime);
        if (!spotLight.enabled)
            batteryFill.gameObject.SetActive(false);
    }

    void ScareEnemies()
    {
        BlindEnemy[] enemies = Object.FindObjectsByType<BlindEnemy>(FindObjectsSortMode.None);

        foreach (BlindEnemy enemy in enemies)
        {
            Vector3 dirToEnemy = enemy.transform.position - spotLight.transform.position;
            float distance = dirToEnemy.magnitude;

            if (distance > scareDistance)
                continue;

            dirToEnemy.Normalize();
            float angleToEnemy = Vector3.Angle(spotLight.transform.forward, dirToEnemy);

            if (angleToEnemy <= scareAngle / 2f)
                enemy.ScaredByLight(spotLight.transform.position);
        }
    }
}
