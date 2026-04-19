using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class EnemyRespawnManager : MonoBehaviour
{
    public static EnemyRespawnManager instance;

    private List<ElementalHealth> _enemies = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Register(ElementalHealth enemy)
    {
        if (enemy.arenaEnemy) return;
        if (!_enemies.Contains(enemy))
            _enemies.Add(enemy);
    }

    public void OnPlayerDeath()
    {
        _enemies.RemoveAll(e => e == null);

        foreach (var enemy in _enemies)
        {
            var root = enemy.transform.parent != null
                ? enemy.transform.parent.gameObject
                : enemy.gameObject;

            root.SetActive(true);
            enemy.ResetLife();
        }
    }
}