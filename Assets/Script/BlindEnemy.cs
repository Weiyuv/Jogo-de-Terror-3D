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
    public float forgetTime = 5f;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Animação")]
    public Animator animator; // Referência ao Animator
    private const string SpeedParam = "Speed"; // Nome do parâmetro float no Animator

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;
    private float timeSinceHeard = 0f;

    // debug
    private bool heardSomething = false;
    private float lastHeardDistance = 0f;
    private float lastEffectiveRange = 0f;

    public bool Chasing => chasing;

    public void StopChase()
    {
        chasing = false;
        heardSomething = false;
        cooldownTimer = cooldown;
        Debug.Log($"{name} teve a perseguição interrompida pelo HidingPlace.");
    }

    void Start()
    {
        if (patrolPoints.Length > 0 && agent != null)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        // cooldown para ouvir sons
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        // Atualiza animação com base na velocidade
        UpdateAnimation();

        if (chasing)
        {
            // Perseguir o som
            timeSinceHeard += Time.deltaTime;
            agent.speed = runSpeed;
            agent.SetDestination(lastHeardPos);

            // Chegou ao destino
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                chasing = false;
                cooldownTimer = cooldown;
                Debug.Log($"{name} chegou na posição do som e parou de correr.");
            }

            // Esqueceu o som
            if (timeSinceHeard >= forgetTime)
            {
                chasing = false;
                heardSomething = false;
                Debug.Log($"{name} esqueceu o som e voltou ao normal.");
            }
        }
        else
        {
            // Patrulha
            if (patrolPoints.Length > 0 && agent != null)
            {
                agent.speed = walkSpeed;

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        float targetSpeed = 0f;

        if (chasing)
            targetSpeed = 1f; // Correndo
        else if (agent.velocity.magnitude > 0.1f)
            targetSpeed = 0.5f; // Andando
        else
            targetSpeed = 0f; // Idle

        animator.SetFloat(SpeedParam, targetSpeed);
    }

    public void HearSound(Vector3 soundPos, float volumeMultiplier)
    {
        if (cooldownTimer > 0) return;
        if (volumeMultiplier <= 0f) return;

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

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            foreach (Transform p in patrolPoints)
            {
                if (p != null)
                    Gizmos.DrawSphere(p.position, 0.2f);
            }
        }
    }
}
