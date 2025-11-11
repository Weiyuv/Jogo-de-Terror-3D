using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : MonoBehaviour
{
    [Header("Configuração")]
    public NavMeshAgent agent;
    public float baseHearingRange = 15f;
    public float runSpeed = 5f;
    public float walkSpeed = 2f;
    public float cooldown = 1f;

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;

    // 🔍 debug
    private bool heardSomething = false;
    private float lastHeardDistance = 0f;
    private float lastEffectiveRange = 0f;

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (chasing)
        {
            agent.speed = runSpeed;
            agent.SetDestination(lastHeardPos);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                chasing = false;
                cooldownTimer = cooldown;
                Debug.Log($"{name} chegou na posição do som.");
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

        // Salva pra debug visual
        lastHeardDistance = distance;
        lastEffectiveRange = effectiveRange;

        if (distance <= effectiveRange)
        {
            lastHeardPos = soundPos;
            chasing = true;
            heardSomething = true;
            Debug.Log($"{name} ouviu som a {distance:F1}m (alcance {effectiveRange:F1}m). Indo investigar!");
        }
        else
        {
            heardSomething = false;
            Debug.Log($"{name} NÃO ouviu (distância {distance:F1}m / alcance {effectiveRange:F1}m).");
        }
    }

    // 🎯 Mostra o alcance auditivo na Scene
    private void OnDrawGizmosSelected()
    {
        // Circulo do alcance de audição
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, baseHearingRange);

        // Mostra posição do som ouvido
        if (heardSomething)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastHeardPos, 0.3f);
            Gizmos.DrawLine(transform.position, lastHeardPos);
        }

        // Mostra o alcance efetivo da última vez que ouviu
        if (lastEffectiveRange > 0)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, lastEffectiveRange);
        }
    }
}
