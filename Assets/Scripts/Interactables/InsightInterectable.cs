using UnityEngine;

public class InsightInteractable : Interactable
{
    [Header("Graphic Child")]
    [SerializeField] private SpriteRenderer graphicRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteAvailable;
    [SerializeField] private Sprite spriteCollected;

    private void Start()
    {
        if (graphicRenderer != null && spriteAvailable != null)
            graphicRenderer.sprite = spriteAvailable;
    }

    public override void Interact()
    {
        base.Interact();
        OnInterect?.Invoke();

        if (graphicRenderer != null && spriteCollected != null)
            graphicRenderer.sprite = spriteCollected;

        canInteract = false;
        OnCantInteract();
    }
}