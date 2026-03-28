using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InsightUI : MonoBehaviour
{
    [Header("XP Ring")]
    public Image xpFillImage;

    [Header("Level")]
    public TextMeshProUGUI levelText;

    [Header("Insight Points")]
    public GameObject insightPointIndicator;

    private Coroutine blinkCoroutine;
    
    private float _lastFill;

    private void Start()
    {
        InsightSystem.instance.onInsightChange += UpdateUI;
        UpdateUI();
    }

    private void OnEnable()
    {
        if (InsightSystem.instance != null)
            UpdateUI();
        else if (xpFillImage != null)
            xpFillImage.fillAmount = _lastFill; // <- reaaplica se o singleton não estiver pronto
    }

    private void OnDestroy()
    {
        if (InsightSystem.instance != null)
            InsightSystem.instance.onInsightChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        if (InsightSystem.instance == null) return;

        if (xpFillImage != null)
        {
            float fill = (float)InsightSystem.instance.currentInsight / InsightSystem.instance.insightToNextLevel;
            xpFillImage.fillAmount = Mathf.Clamp01(fill);
            _lastFill = Mathf.Clamp01(fill);
        }

        if (levelText != null)
            levelText.text = InsightSystem.instance.currentLevel.ToString();

        if (insightPointIndicator != null)
        {
            bool hasPoints = InsightSystem.instance.insightPoints > 0;
            insightPointIndicator.SetActive(hasPoints);

            if (hasPoints && blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(BlinkLoop());
            else if (!hasPoints && blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
        }
    }

    private IEnumerator BlinkLoop()
    {
        Image img = insightPointIndicator.GetComponent<Image>();
        if (img == null) yield break;

        while (true)
        {
            yield return Fade(img, 1f, 0f, 0.6f);
            yield return Fade(img, 0f, 1f, 0.6f);
        }
    }

    private IEnumerator Fade(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = img.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            img.color = c;
            yield return null;
        }

        c.a = to;
        img.color = c;
    }
}