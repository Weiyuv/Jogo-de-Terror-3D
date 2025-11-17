using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : MonoBehaviour
{
    [Header("Configuração")]
    public NavMeshAgent agent;
    public float baseHearingRange = 15f;   // alcance de audição base
    public float runSpeed = 5f;            // velocidade ao perseguir
    public float walkSpeed = 2f;           // velocidade normal
    public float cooldown = 1f;            // tempo entre ouvir sons
    public float forgetTime = 5f;          // tempo pra esquecer o som

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;
    private float timeSinceHeard = 0f;

    // 🔍 debug
    private bool heardSomething = false;
    private float lastHeardDistance = 0f;
    private float lastEffectiveRange = 0f;

    // -------------------------------
    // ACESSO PÚBLICO PARA HidingPlace
    // -------------------------------
    public bool Chasing => chasing; // propriedade pública para saber se está perseguindo

    public void StopChase()
    {
        chasing = false;
        heardSomething = false;
        cooldownTimer = cooldown;
        Debug.Log($"{name} teve a perseguição interrompida pelo HidingPlace.");
    }

    // -------------------------------
    // SISTEMA ORIGINAL
    // -------------------------------
    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (chasing)
        {
            timeSinceHeard += Time.deltaTime;

            // Corre enquanto estiver perseguindo
            agent.speed = runSpeed;
            agent.SetDestination(lastHeardPos);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                chasing = false;
                cooldownTimer = cooldown;
                Debug.Log($"{name} chegou na posição do som e parou de correr.");
            }

            if (timeSinceHeard >= forgetTime)
            {
                chasing = false;
                heardSomething = false;
                Debug.Log($"{name} esqueceu o som e voltou ao normal.");
            }
        }
        else
        {
            agent.speed = walkSpeed;
        }
    }

    public void HearSound(Vector3 soundPos, float volumeMultiplier)
    {
        if (cooldownTimer > 0) return;

        float distance = Vector3.Distance(transform.position, soundPos);
        float effectiveRange = baseHearingRange * volumeMultiplier;

        lastHeardDistance = distance;
        lastEffectiveRange = effectiveRange;

        if (distance <= effectiveRange)
        {
            lastHeardPos = soundPos;
            chasing = true;
            heardSomething = true;
            timeSinceHeard = 0f;

            Debug.Log($"{name} ouviu som a {distance:F1}m (alcance {effectiveRange:F1}m). CORRENDO até o som!");
        }
        else
        {
            heardSomething = false;
            Debug.Log($"{name} NÃO ouviu (distância {distance:F1}m / alcance {effectiveRange:F1}m).");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, baseHearingRange);

        if (heardSomething)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastHeardPos, 0.3f);
            Gizmos.DrawLine(transform.position, lastHeardPos);
        }

        if (lastEffectiveRange > 0)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, lastEffectiveRange);
        }
    }
}
