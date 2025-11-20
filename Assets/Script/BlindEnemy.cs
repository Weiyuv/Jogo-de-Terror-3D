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
    public Animator animator;
    private const string SpeedParam = "Speed";

    [Header("Medo da luz")]
    public float fearDuration = 3f; // tempo que foge da luz
    private float fearTimer = 0f;
    private Vector3 fearDirection;

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;
    private float timeSinceHeard = 0f;

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
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        // cooldown para ouvir sons
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        // Atualiza animação
        UpdateAnimation();

        // Se com medo da luz
        if (fearTimer > 0)
        {
            fearTimer -= Time.deltaTime;
            agent.speed = runSpeed;
            agent.SetDestination(transform.position + fearDirection); // foge
            return;
        }

        if (chasing)
        {
            timeSinceHeard += Time.deltaTime;
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

        if (fearTimer > 0 || chasing)
            targetSpeed = 1f; // correndo
        else if (agent.velocity.magnitude > 0.1f)
            targetSpeed = 0.5f; // andando
        else
            targetSpeed = 0f; // idle

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
            Debug.Log($"{name} ouviu som a {distance:F1}m. CORRENDO até o som!");
        }
        else
        {
            heardSomething = false;
            Debug.Log($"{name} NÃO ouviu o som.");
        }
    }

    // Chamado para “assustar” o inimigo pela luz
    public void ScaredByLight(Vector3 lightPos)
    {
        fearTimer = fearDuration;
        fearDirection = (transform.position - lightPos).normalized * 5f; // foge 5 metros
        chasing = false;
        heardSomething = false;
        Debug.Log($"{name} tem medo da luz e está fugindo!");
    }
}
