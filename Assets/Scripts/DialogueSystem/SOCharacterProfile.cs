using Characters;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Dialogue/CharacterProfile")]
public class SOCharacterProfile : ScriptableObject
{
    public CharacterName character;
    public string description;
    public Sprite characterImage;
    public AudioClip[] dialogueAudioClips;
    public string[] characterDialogues;
    [HideInInspector] public int currentDialogueIndex;
}
