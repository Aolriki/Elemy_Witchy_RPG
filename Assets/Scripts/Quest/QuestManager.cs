using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public QuestData initialQuest;

    [SerializeField] private QuestData currentQuest;
    public QuestData CurrentQuest => currentQuest;

    public List<QuestData> completedQuests = new List<QuestData>();

    public event System.Action<QuestData> OnQuestChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentQuest = initialQuest;
        OnQuestChanged?.Invoke(currentQuest);
    }

    public void ActivateNewQuest(QuestData nextQuest)
    {
        if (nextQuest == null) return;
        if (nextQuest == currentQuest) return;
        if (completedQuests.Contains(nextQuest)) return;

        CompleteQuest();

        currentQuest = nextQuest;
        OnQuestChanged?.Invoke(currentQuest);
    }

    private void CompleteQuest()
    {
        if (currentQuest == null) return;
        completedQuests.Add(currentQuest);
    }
}