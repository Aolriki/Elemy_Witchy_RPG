using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInterect;
    protected SpriteRenderer[] spriteRenderers;
    public GameObject interactionIcon;
    private Tween currTween;
    private bool interacting;
    public bool canInteract = true; // protected -> public

    private void OnEnable()
    {
        if (currTween != null)
        {
            currTween.Play();
        }
        else
        {
            FloatEffect();
        }
    }

    private void OnDisable()
    {
        if (currTween != null)
        {
            currTween.Pause();
        }
    }

    protected void FloatEffect()
    {
        float rand = Random.Range(0.8f, 1.2f);
        currTween = interactionIcon.transform.DOLocalMoveY(interactionIcon.transform.localPosition.y - 0.4f, rand)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }

    public void ChangeColorOnMouseDown(int changeColor)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.SetInt("_ChangeColorOnMouseDown", changeColor);
        }
    }

    public virtual void Interact()
    {
        if (!canInteract) return;
        Debug.Log("Interact");
    }

    public virtual void OnCanInteract()
    {
        if (!canInteract) return;
        interactionIcon.SetActive(true);
        interacting = true;
    }

    public virtual void OnCantInteract()
    {
        interactionIcon.SetActive(false);
        interacting = false;
    }

    private void OnDestroy()
    {
        if (interacting)
        {
            //ScreenManager.Instance.ChangeInteract(false);
        }
    }
}