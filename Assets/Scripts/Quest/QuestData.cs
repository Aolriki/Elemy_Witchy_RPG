using UnityEngine;
public enum QuestType { InterectCheck, ColliderCheck, KillCheck }
[CreateAssetMenu(fileName = "QuestSO", menuName = "Quest")]

public class QuestData : ScriptableObject

{

    public string questName;



    [TextArea(15, 20)]

    public string description = "";

}