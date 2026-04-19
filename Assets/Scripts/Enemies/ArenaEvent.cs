using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ArenaEvent : MonoBehaviour
{
    [Header("Entry Collider")]
    [Tooltip("Collider 2D trigger que inicia a arena. Pode ser filho deste objeto.")]
    public Collider2D entryCollider;

    [Header("Waves — em ordem")]
    public List<ConditionalSpawner> waveSpawners;

    [Header("Reward")]
    public ConditionalSpawner rewardSpawner;

    [Header("Events")]
    public UnityEvent OnArenaStarted;
    public UnityEvent OnArenaCompleted;
    public UnityEvent OnArenaReset;

    private int _currentWave = 0;
    private bool _arenaStarted = false;
    private bool _arenaCompleted = false;

    private void Awake()
    {
        DeactivateAllEnemies();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_arenaStarted || _arenaCompleted) return;

        other.GetComponent<Health>().OnDeath.AddListener(OnPlayerDeath);
        StartArena();
    }

    private void StartArena()
    {
        _arenaStarted = true;
        _currentWave = 0;
        OnArenaStarted?.Invoke();
        ActivateWave(_currentWave);
    }

    private void ActivateWave(int index)
    {
        if (index >= waveSpawners.Count)
        {
            CompleteArena();
            return;
        }

        var spawner = waveSpawners[index];
        spawner.ResetSpawner();
        spawner.Activate();

        // Detecta quando todos os inimigos dessa wave foram derrotados
        var enemies = spawner.GetComponentsInChildren<ElementalHealth>(true);
        StartCoroutine(WaitForWaveClear(enemies));
    }

    private System.Collections.IEnumerator WaitForWaveClear(ElementalHealth[] enemies)
    {
        yield return null; // espera um frame para os inimigos ativarem

        while (true)
        {
            bool allDefeated = true;
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    allDefeated = false;
                    break;
                }
            }

            if (allDefeated) break;
            yield return null;
        }

        _currentWave++;
        ActivateWave(_currentWave);
    }

    private void CompleteArena()
    {
        _arenaCompleted = true;
        Player.instance.GetComponent<Health>().OnDeath.RemoveListener(OnPlayerDeath);

        if (rewardSpawner != null)
        {
            rewardSpawner.ResetSpawner();
            rewardSpawner.Activate();
        }
        OnArenaCompleted?.Invoke();
    }

    public void OnPlayerDeath()
    {
        if (_arenaCompleted) return;
        Player.instance.GetComponent<Health>().OnDeath.RemoveListener(OnPlayerDeath);
        StopAllCoroutines();
        _arenaStarted = false;
        _currentWave = 0;
        DeactivateAllEnemies();
        OnArenaReset?.Invoke();
    }

    private void DeactivateAllEnemies()
    {
        foreach (var spawner in waveSpawners)
        {
            spawner.ResetSpawner();
            foreach (var enemy in spawner.GetComponentsInChildren<ElementalHealth>(true))
            {
                enemy.ResetLife();
                enemy.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}