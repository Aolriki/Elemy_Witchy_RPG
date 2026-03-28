using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthSliderUpdater : MonoBehaviour
{
    [SerializeField] Slider mainSlider;
    [SerializeField] Slider subSlider;

    public void UpdateHealthUI(float health)
    {
        mainSlider.value = health;
        if(subSlider != null) subSlider.DOValue(health, 1.5f).SetEase(Ease.InOutSine);
    }
}
