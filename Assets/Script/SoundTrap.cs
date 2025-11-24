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

    private bool triggered = false;

    void Update()
    {
        if (triggered || player == null || trapCollider == null) return;

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

        if (trapAudio != null)
            trapAudio.Play();

        if (blindEnemies == null || blindEnemies.Length == 0)
            blindEnemies = Object.FindObjectsByType<BlindEnemy>(FindObjectsSortMode.None);

        foreach (BlindEnemy enemy in blindEnemies)
            if (enemy != null)
                enemy.HearSound(transform.position, soundRangeMultiplier);

        Debug.Log($"{name} foi acionada pelo Player usando OverlapBox!");
    }
}
