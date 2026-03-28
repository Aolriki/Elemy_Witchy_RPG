using UnityEngine;
using System;

public class InsightSystem : MonoBehaviour
{
    public static InsightSystem instance;

    [Header("Insight")]
    public int currentInsight = 0;
    public int insightToNextLevel = 100;

    [Header("Level")]
    public int currentLevel = 1;
    public int maxLevel = 15;
    public int insightPoints;

    [Header("Tuning")]
    public float levelScaling = 2.5f; // Multiplicador de crescimento por nível

    public event Action onInsightChange;
    public event Action onLevelUp;

    private void Awake()
    {
        insightPoints = 0;
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddInsight(int amount)
    {
        if (currentLevel >= maxLevel) return; // Já está no nível máximo

        currentInsight += amount;
        Debug.Log($"Insight gained: +{amount}. Current: {currentInsight}/{insightToNextLevel}");

        onInsightChange?.Invoke();

        // Verifica level up em loop, caso o insight ganho seja suficiente para múltiplos níveis
        while (currentInsight >= insightToNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentInsight -= insightToNextLevel;
        currentLevel++;
        insightPoints++;

        // 100 + nivelAtual * 2.5
        insightToNextLevel = 100 + Mathf.RoundToInt(currentLevel * levelScaling);

        Debug.Log($"Level Up! Level: {currentLevel} | Points: {insightPoints} | Next level: {insightToNextLevel}");

        onLevelUp?.Invoke();
        onInsightChange?.Invoke();

        AudioManager.Instance.PlayEffect(SFXID.LevelUp);
    }

    public void SpendInsightPoint()
    {
        if (insightPoints <= 0)
        {
            Debug.Log("No insight points to spend.");
            return;
        }
        insightPoints--;
        Debug.Log($"Insight point spent. Remaining: {insightPoints}");
        onInsightChange?.Invoke();
        return;
    }

    public bool HasEnoughInsight()
    {
        return insightPoints >= 1;
    }
}
