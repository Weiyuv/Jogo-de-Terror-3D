using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : MonoBehaviour
{
    [Header("Configuração")]
    public NavMeshAgent agent;
    public float baseHearingRange = 15f;
    public float cooldown = 1f;
    public float forgetTime = 5f;

    [Header("Velocidade")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Animação")]
    public Animator animator;
    private const string SpeedParam = "Speed";

    [Header("Medo da luz")]
    public float fearDuration = 3f;
    private float fearTimer = 0f;
    private Vector3 fearDirection;

    [Header("Pulo do sapo")]
    public float hopHeight = 0.2f;
    public float hopCooldown = 0.3f;
    private float hopTimer = 0f;

    [Header("Player Detection")]
    public float playerDestroyDistance = 1f; // usado só para definir o tamanho do SphereCollider

    [Header("Som do inimigo")]
    public AudioSource heardSoundAudio;

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;
    private float timeSinceHeard = 0f;
    private bool heardSomething = false;

    private GameObject player;

    public bool Chasing => chasing;

    void Start()
    {
        if (agent != null)
        {
            agent.updatePosition = true;
            agent.updateRotation = false;
        }

        if (patrolPoints.Length > 0 && agent != null)
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        player = GameObject.FindWithTag("Player");

        // Configurar collider de trigger para matar o player
        SphereCollider attackCollider = gameObject.AddComponent<SphereCollider>();
        attackCollider.isTrigger = true;
        attackCollider.radius = playerDestroyDistance;
    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
        if (hopTimer > 0) hopTimer -= Time.deltaTime;

        HandleMovement();
        RotateTowardsMovementDirection();
        HandleHopVertical();
        UpdateAnimation();
    }

    private void HandleMovement()
    {
        if (agent == null) return;

        if (fearTimer > 0)
        {
            fearTimer -= Time.deltaTime;
            agent.speed = runSpeed;
            agent.SetDestination(transform.position + fearDirection);
            return;
        }

        if (chasing)
        {
            timeSinceHeard += Time.deltaTime;
            agent.speed = runSpeed;
            agent.SetDestination(lastHeardPos);

            if (!agent.pathPending && Vector3.Distance(transform.position, lastHeardPos) <= agent.stoppingDistance)
            {
                chasing = false;
                cooldownTimer = cooldown;
            }

            if (timeSinceHeard >= forgetTime)
            {
                chasing = false;
                heardSomething = false;
            }
        }
        else
        {
            agent.speed = walkSpeed;

            if (patrolPoints.Length > 0 &&
                (!agent.hasPath || agent.pathPending || agent.remainingDistance <= agent.stoppingDistance))
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }
    }

    private void RotateTowardsMovementDirection()
    {
        if (agent == null || agent.velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 moveDir = agent.velocity;
        moveDir.y = 0;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            float rotationSpeed = 180f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleHopVertical()
    {
        if (hopTimer > 0) return;

        transform.position += Vector3.up * hopHeight;
        hopTimer = hopCooldown;
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        float targetSpeed = 0f;

        if (fearTimer > 0 || chasing)
            targetSpeed = 1f;
        else if (agent.velocity.magnitude > 0.01f)
            targetSpeed = 0.5f;
        else
            targetSpeed = 0f;

        animator.SetFloat(SpeedParam, targetSpeed);
    }

    public void HearSound(Vector3 soundPos, float volumeMultiplier)
    {
        if (cooldownTimer > 0 || volumeMultiplier <= 0f) return;

        float distance = Vector3.Distance(transform.position, soundPos);
        float effectiveRange = baseHearingRange * volumeMultiplier;

        if (distance <= effectiveRange)
        {
            bool wasNotChasing = !chasing;

            lastHeardPos = soundPos;
            chasing = true;
            heardSomething = true;
            timeSinceHeard = 0f;

            if (wasNotChasing && heardSoundAudio != null)
                heardSoundAudio.Play();
        }
        else
        {
            heardSomething = false;
        }
    }

    public void ScaredByLight(Vector3 lightPos)
    {
        fearTimer = fearDuration;
        fearDirection = (transform.position - lightPos).normalized * 5f;
        chasing = false;
        heardSomething = false;
    }

    // =========================
    // Trigger para destruir player
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{name} atacou o Player! PLAYER MORTO!");
            Destroy(other.gameObject);
        }
    }
}
