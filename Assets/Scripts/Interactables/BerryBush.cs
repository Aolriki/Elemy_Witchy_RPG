using UnityEngine;

public class BerryBush : Interactable
{
    [SerializeField] private float healAmount = 3f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite availableBush;
    [SerializeField] private Sprite unavailableBush;

    public override void Interact()
    {
        base.Interact();

        Player.instance.health.Heal(healAmount);

        spriteRenderer.sprite = unavailableBush;

        canInteract = false;
        OnCantInteract();
    }
}
