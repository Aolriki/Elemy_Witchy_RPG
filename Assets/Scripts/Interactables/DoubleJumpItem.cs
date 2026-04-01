using UnityEngine;
using UnityEngine.Events;

public class DoubleJumpItem : MonoBehaviour
{
    [Header("Graphic Child")]
    [SerializeField] private SpriteRenderer graphicRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteAvailable;
    [SerializeField] private Sprite spriteCollected;

    [Header("References")]
    public GameObject targetObjToDestroy;
    public DialogueCharacter dialogueCharacter;

    private void Start()
    {
        if (graphicRenderer != null && spriteAvailable != null)
            graphicRenderer.sprite = spriteAvailable;

        if (dialogueCharacter != null)
            dialogueCharacter.OnEndDialogue.AddListener(GrantDoubleJump);
    }

    private void OnDestroy()
    {
        if (dialogueCharacter != null)
            dialogueCharacter.OnEndDialogue.RemoveListener(GrantDoubleJump);
    }

    public void GrantDoubleJump()
    {
        if (graphicRenderer != null && spriteCollected != null)
            graphicRenderer.sprite = spriteCollected;

        Player.instance.PhysicsContext.doubleJumpUnlocked = true;

        if (dialogueCharacter != null)
            dialogueCharacter.OnEndDialogue.RemoveListener(GrantDoubleJump);
    }

    public void DestroyEvent()
    {
        Destroy(targetObjToDestroy);
    }
}