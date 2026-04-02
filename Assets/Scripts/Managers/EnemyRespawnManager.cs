using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    public static EnemyRespawnManager instance;

    private System.Collections.Generic.List<ElementalHealth> registeredEnemies = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Register(ElementalHealth enemy)
    {
        if (!registeredEnemies.Contains(enemy))
            registeredEnemies.Add(enemy);
    }

    public void OnPlayerDeath()
    {
        foreach (var enemy in registeredEnemies)
        {
            if (enemy == null) continue;
            enemy.transform.parent.gameObject.SetActive(true);
            enemy.ResetLife();
        }
    }
}