using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public float baseHearingRange = 15f;
    public float runSpeed = 5f;
    public float walkSpeed = 2f;
    public float cooldown = 1f;

    private Vector3 lastHeardPos;
    private bool chasing = false;
    private float cooldownTimer = 0f;

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

        if (distance <= effectiveRange)
        {
            lastHeardPos = soundPos;
            chasing = true;
        }
    }
}
