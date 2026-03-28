using UnityEngine;

[CreateAssetMenu(fileName = "NewResponseAction", menuName = "Dialogue/ResponseAction")]
public abstract class DialogueAction : ScriptableObject
{
    public abstract void Execute();
}
