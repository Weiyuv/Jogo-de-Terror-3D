using UnityEngine;

public class NormalFlash : MonoBehaviour
{
    [Header("Configuração da luz")]
    public Light spotLight;      // sua Spot Light
    public bool startOn = false; // começa ligada?

    void Start()
    {
        if (spotLight != null)
            spotLight.enabled = startOn;
    }

    void Update()
    {
        if (spotLight == null) return;

        // Liga/desliga com F
        if (Input.GetKeyDown(KeyCode.F))
        {
            spotLight.enabled = !spotLight.enabled;
        }
    }
}
