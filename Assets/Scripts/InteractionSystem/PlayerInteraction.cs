using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction instance;

    public List<Interactable> interactables;

    private Interactable nearestInteractable;

    public bool canInteract = true;

    private void Awake()
    {
        interactables = new List<Interactable>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance = this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (!canInteract) return;

        CheckNearestInteractable();
    }

    public void Interact()
    {
        if (!canInteract) return;

        if (nearestInteractable != null)
        {
            //AudioManager.Instance.PlayEffect("Interaction");
            nearestInteractable.Interact();
        }
    }

    public void CheckNearestInteractable()
    {
        if (interactables.Count == 0)
        {
            if (nearestInteractable != null) nearestInteractable.OnCantInteract();
            nearestInteractable = null;
            return;
        }

        float minDistance = float.MaxValue;
        Interactable closest = null;
        Vector3 playerPosition = transform.position;

        foreach (Interactable interactable in interactables.ToArray())
        {
            if (interactable == null)
            {
                interactables.Remove(interactable);
                continue;
            }

            float distance = Vector3.Distance(playerPosition, interactable.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = interactable;
            }
        }
        if (nearestInteractable != closest)
        {
            if (nearestInteractable != null) nearestInteractable.OnCantInteract();
            closest.OnCanInteract();
        }
        nearestInteractable = closest;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (interactable != null)
        {
            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (interactable != null)
        {
            interactables.Remove(interactable);
        }
    }
}
