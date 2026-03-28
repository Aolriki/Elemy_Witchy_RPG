using Characters;
using UnityEngine;

#region Dialogue List Structure

[System.Serializable]
public class Sentence
{
    public CharacterName talker;
    public string sentenceText;
    public DialogueAction sentenceAction;
}

[System.Serializable]
public class Response
{
    public string responseText;
    public string GoToDialogue;
    public DialogueAction responseAction;
}

#endregion

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class SODialogue : ScriptableObject
{
    public Sentence[] sentences;
    public Response[] responses;
}
