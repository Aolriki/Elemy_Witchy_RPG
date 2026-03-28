using UnityEngine;
public class DoubleJumpItem : Interactable
{
    public GameObject targetObjToDestroy;
    public override void Interact()
    {
        base.Interact();
        canInteract = false; // impede dupla detecção antes do Destroy
        Player.instance.PhysicsContext.doubleJumpUnlocked = true;
        Destroy(gameObject);
    }

    public void DestroyEvent()
    {
        Destroy(targetObjToDestroy);
    }
}
