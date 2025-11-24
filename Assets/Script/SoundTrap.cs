using UnityEngine;

public class SoundTrap : MonoBehaviour
{
    [Header("Som da armadilha")]
    public AudioSource trapAudio;

    [Header("Inimigos cegos")]
    public BlindEnemy[] blindEnemies;

    [Header("Player")]
    public GameObject player;

    [Header("Collider da armadilha")]
    public Collider trapCollider; // arraste o collider do trap aqui

    [Header("Alcance do som")]
    [Range(0f, 5f)]
    public float soundRangeMultiplier = 1f;

    [Header("Delay de reutilização")]
    public float reuseDelay = 2f; // tempo antes de poder ser acionada novamente

    private bool triggered = false;
    private float cooldownTimer = 0f;

    void Update()
    {
        if (player == null || trapCollider == null) return;

        // Atualiza o cooldown
        if (triggered)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                triggered = false; // armadilha pronta para ser acionada novamente
            else
                return; // ainda no cooldown, não verifica colisão
        }

        // Cria um overlap usando o collider da armadilha
        Collider[] hits = Physics.OverlapBox(trapCollider.bounds.center,
                                             trapCollider.bounds.extents,
                                             trapCollider.transform.rotation);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == player)
            {
                TriggerTrap();
                break;
            }
        }
    }

    private void TriggerTrap()
    {
        triggered = true;
        cooldownTimer = reuseDelay;

        // Toca o áudio da armadilha
        if (trapAudio != null && trapAudio.clip != null)
        {
            trapAudio.Play();
        }

        // Se a lista de inimigos não estiver atribuída, pega todos na cena
        if (blindEnemies == null || blindEnemies.Length == 0)
            blindEnemies = Object.FindObjectsByType<BlindEnemy>(FindObjectsSortMode.None);

        foreach (BlindEnemy enemy in blindEnemies)
        {
            if (enemy != null)
                enemy.HearSound(transform.position, soundRangeMultiplier);
        }

        Debug.Log($"{name} foi acionada pelo Player! Armadilha em cooldown por {reuseDelay}s.");
    }

    // Para visualizar o alcance da armadilha no editor
    private void OnDrawGizmosSelected()
    {
        if (trapCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.matrix = trapCollider.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, trapCollider.bounds.size);
    }
}
