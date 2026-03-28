using UnityEngine;

public class InsightReward : MonoBehaviour
{
    public enum InsightRewardType { Small, Mid, Big }

    [Header("Insight Reward")]
    public InsightRewardType insightReward = InsightRewardType.Small;

    private static readonly int[] insightValues = { 35, 50, 80 };

    public void GiveInsight()
    {
        InsightSystem.instance.AddInsight(insightValues[(int)insightReward]);
        Debug.Log($"Insight granted on death: {insightValues[(int)insightReward]}");
    }
}
