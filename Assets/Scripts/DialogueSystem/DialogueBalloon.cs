using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueBalloon : MonoBehaviour
{
    private RectTransform rect;
    private void Awake()
    {
        rect = invisSentenceTMP.rectTransform;
    }
    public string currentSentenceText;

    public RectTransform SpeechBubbleRectTransform;

    public TextMeshProUGUI invisSentenceTMP;
    public TextMeshProUGUI sentenceTMP;
    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    [HideInInspector] public UnityEvent OnTypingOverEvent;


    public float maxWidth = 750f;

    #region Main Methods
    public void UpdateText(string newIncommingText)
    {
        currentSentenceText = newIncommingText;

        SetUpInvisibleFullText();
        UpdateSize();
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence());
    }
    public void FullSentence()
    {
        StopCoroutine(typingCoroutine);
        sentenceTMP.text = currentSentenceText;
    }
    #endregion



    private void SetUpInvisibleFullText()
    {
        invisSentenceTMP.text = currentSentenceText;
    }
    private void UpdateSize()
    {

        invisSentenceTMP.ForceMeshUpdate();

        Vector2 preferred = invisSentenceTMP.GetPreferredValues(currentSentenceText);

        //Calcula Largura baseado no tamanho do texto e largura maxima
        float width = Mathf.Min(preferred.x, maxWidth);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        //Agora recalcula altura baseado na largura aplicada
        float height = invisSentenceTMP.GetPreferredValues(currentSentenceText, width, 0).y;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        // Força atualização do balão
        LayoutRebuilder.ForceRebuildLayoutImmediate(SpeechBubbleRectTransform);

    }
    private IEnumerator TypeSentence()
    {
        sentenceTMP.text = "";

        foreach (char letter in currentSentenceText.ToCharArray())
        {
            sentenceTMP.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        OnTypingOverEvent?.Invoke();
    }



}
