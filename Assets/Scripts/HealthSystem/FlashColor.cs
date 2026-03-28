using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FlashColor : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;

    [SerializeField] private float duration;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Material originalMaterial;

    private Coroutine flashRoutine;

    private Tween flashTween;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        originalMaterial = spriteRenderer.material;

        flashMaterial = new Material(flashMaterial);
    }

    public void Flash(Color color)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color color)
    {
        spriteRenderer.material = flashMaterial;

        flashMaterial.SetColor("_BaseColor", color);

        yield return new WaitForSeconds(duration);

        spriteRenderer.material = originalMaterial;

        flashRoutine = null;
    }

    public void FlashLoop(Color color, float totalDuration)
    {
        if (flashTween != null && flashTween.IsActive()) flashTween.Kill();

        flashTween = spriteRenderer.DOColor(color, 0.2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);

        StartCoroutine(StopFlashLoopAfterTime(totalDuration));
    }

    private IEnumerator StopFlashLoopAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (flashTween != null && flashTween.IsActive())
        {
            flashTween.Kill();
            flashTween = null;
        }

        spriteRenderer.color = Color.white;
    }
}
