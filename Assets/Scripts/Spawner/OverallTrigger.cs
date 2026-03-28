using UnityEngine;
using UnityEngine.Events;

public class OverallTrigger : Interactable
{
    public override void Interact()
    {
        base.Interact();
        OnInterect?.Invoke();

        canInteract = false;
        OnCantInteract();
    }

}
