using UnityEngine;
using UnityEngine.Events;


public class OverallTrigger : Interactable
{

    public UnityEvent OnCollider;
    public override void Interact()
    {
        base.Interact();
        OnInterect?.Invoke();

        canInteract = false;
        OnCantInteract();
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Boss Fight Começa!");
            OnCollider?.Invoke();
            canInteract = false;
        }
    }
}
