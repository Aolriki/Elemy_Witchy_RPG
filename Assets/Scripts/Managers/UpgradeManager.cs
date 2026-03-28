using UnityEngine;
using System;

public enum UpgradeLevel { None, Level1, Level2, level3, level4, Max }

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public event Action onUpgrade;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start() => _insight = InsightSystem.instance;

    private InsightSystem _insight;

    private UpgradeLevel _elemyTimerLevel = UpgradeLevel.None;
    private UpgradeLevel _damageLevel = UpgradeLevel.None;
    private UpgradeLevel _maxHealthLevel = UpgradeLevel.None;

    public UpgradeLevel ElemyTimerLevel => _elemyTimerLevel;
    public UpgradeLevel DamageLevel => _damageLevel;
    public UpgradeLevel MaxHealthLevel => _maxHealthLevel;

    // Bônus incrementais por nível (0, 1, 2, 3)
    // O valor base vem do arquétipo escolhido 
    [Header("Bônus por Nível: Elemy Timer")]
    [SerializeField] private float[] elemyTimerBonus = { 0f, 1f, 1f, 1f, 1f, 1f };

    [Header("Bônus por Nível: Dano")]
    [SerializeField] private int[] damageBonus = { 0, 1, 1, 1, 1, 1 };

    [Header("Bônus por Nível: Vida Máxima")]
    [SerializeField] private float[] maxHealthBonus = { 0f, 1f, 1f, 1f, 1f, 1f };

    // Bônus atual baseado no nível
    public float ElemyTimerBonus => elemyTimerBonus[(int)_elemyTimerLevel];
    public int DamageBonus => damageBonus[(int)_damageLevel];
    public float MaxHealthBonus => maxHealthBonus[(int)_maxHealthLevel];

    public bool TryUpgradeElemyTimer() => TryUpgrade(ref _elemyTimerLevel);
    public bool TryUpgradeDamage() => TryUpgrade(ref _damageLevel);
    public bool TryUpgradeMaxHealth() => TryUpgrade(ref _maxHealthLevel);

    public bool CanUpgrade(UpgradeLevel level) =>
        level != UpgradeLevel.Max && _insight.HasEnoughInsight();

    private bool TryUpgrade(ref UpgradeLevel current)
    {
        if (!CanUpgrade(current)) return false;
        _insight.SpendInsightPoint();
        current = (UpgradeLevel)((int)current + 1);
        onUpgrade?.Invoke();
        return true;
    }
}