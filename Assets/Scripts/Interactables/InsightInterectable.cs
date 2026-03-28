using UnityEngine;
using UnityEngine.Events;

public class InsightInteractable : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static readonly Color ColorAvailable = new Color(1f, 0.85f, 0.1f);
    private static readonly Color ColorCollected = new Color(0.75f, 0.75f, 0.75f);

    private void Start()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = ColorAvailable;
    }

    public override void Interact()
    {
        base.Interact();
        OnInterect?.Invoke();

        if (spriteRenderer != null)
            spriteRenderer.color = ColorCollected;

        canInteract = false;
        OnCantInteract();
    }
}
