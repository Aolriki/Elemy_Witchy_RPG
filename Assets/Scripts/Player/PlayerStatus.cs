using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    public event Action onStatsChanged;

    public float MaxHealth { get; private set; }
    public int Damage { get; private set; }
    public float ElemyTimer { get; private set; }

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        ArchetypeManager.Instance.onArchetypeSelected += RecalculateStats;
        UpgradeManager.Instance.onUpgrade += RecalculateStats;
    }

    private void OnDestroy()
    {
        if (ArchetypeManager.Instance != null)
            ArchetypeManager.Instance.onArchetypeSelected -= RecalculateStats;

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.onUpgrade -= RecalculateStats;
    }

    private void RecalculateStats()
    {
        if (!ArchetypeManager.Instance.HasArchetype) return;

        ArchetypeData archetype = ArchetypeManager.Instance.CurrentArchetype;

        ElemyTimer = archetype.elemyTimer + UpgradeManager.Instance.ElemyTimerBonus;
        Damage = archetype.damage + UpgradeManager.Instance.DamageBonus;
        MaxHealth = archetype.maxHealth + UpgradeManager.Instance.MaxHealthBonus;

        IsReady = true; // <- marca como pronto após o primeiro cálculo
        onStatsChanged?.Invoke();
    }
}