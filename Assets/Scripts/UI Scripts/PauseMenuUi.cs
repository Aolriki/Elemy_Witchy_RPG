using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject systemPage;
    [SerializeField] private GameObject questPage;
    [SerializeField] private GameObject upgradePage;

    [Header("Page Indicators")]
    [SerializeField] private GameObject pageIndicatorSistema;
    [SerializeField] private GameObject pageIndicatorQuest;
    [SerializeField] private GameObject pageIndicatorAtributos;

    [Header("Navigation Buttons")]
    [SerializeField] private Button buttonSistema;
    [SerializeField] private Button buttonQuest;
    [SerializeField] private Button buttonAtributos;

    [Header("Foco Inicial")]
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject firstSystemSelected;

    // Upgrade Page

    [Header("Upgrade - Circular Sliders")]
    [SerializeField] private Slider elemyTimerSlider;
    [SerializeField] private Slider damageSlider;
    [SerializeField] private Slider maxHealthSlider;

    [Header("Upgrade - Icons")]
    [SerializeField] private Image elemyTimerIcon;
    [SerializeField] private Image damageIcon;
    [SerializeField] private Image maxHealthIcon;

    [Header("Upgrade - Pins")]
    [SerializeField] private GameObject pin_ElemyTimer;
    [SerializeField] private GameObject pin_Damage;
    [SerializeField] private GameObject pin_MaxHealth;

    [Header("Upgrade - UI")]
    [SerializeField] private TextMeshProUGUI insightPointsText;
    [SerializeField] private TextMeshProUGUI noInsightText;
    [SerializeField] private TextMeshProUGUI timerLevelText;
    [SerializeField] private TextMeshProUGUI damageLevelText;
    [SerializeField] private TextMeshProUGUI healthLevelText;

    [Header("Upgrade - Blink Settings")]
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float blinkMinAlpha = 0.2f;
    [SerializeField] private float blinkMaxAlpha = 1f;

    [Header("Upgrade - Icon Dim Settings")]
    [SerializeField] private float iconDimAlpha = 150f / 255f;
    [SerializeField] private float iconFullAlpha = 1f;

    [Header("Quest Display")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;


    private const int MAX_LEVEL = 5;
    private int _selectedIndex = 0;
    private Coroutine _blinkCoroutine;
    private GameObject[] _pins;
    private Image[] _pinImages;

    // Estado geral

    private bool _isPageOpen = false;
    private string _currentPage = "";

    // Lifecycle

    private void Start()
    {
        _pins = new GameObject[] { pin_ElemyTimer, pin_Damage, pin_MaxHealth };
        _pinImages = new Image[]
        {
            pin_ElemyTimer.GetComponent<Image>(),
            pin_Damage.GetComponent<Image>(),
            pin_MaxHealth.GetComponent<Image>()
        };

        InsightSystem.instance.onInsightChange += RefreshUpgradeUI;
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        ClosePage();

        // Assina o evento de troca de quest e atualiza a UI com a quest atual
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestChanged += UpdateQuestUI;
            UpdateQuestUI(QuestManager.Instance.CurrentQuest);
        }
        AudioManager.Instance.PlayEffect(SFXID.OpenPause);
    }

    private void OnDisable()
    {
        if (InsightSystem.instance != null)
            InsightSystem.instance.onInsightChange -= RefreshUpgradeUI;

        StopBlinking();

        // Cancela a assinatura do evento de quest
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestChanged -= UpdateQuestUI;

        AudioManager.Instance.PlayEffect(SFXID.OpenPause);
    }

    // Quest Display

    private void UpdateQuestUI(QuestData quest)
    {
        if (quest == null) return;
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.description;
    }

    // Navegação de páginas

    public void OpenPage(string pageName)
    {
        DeactivateAllPages();
        DeactivateAllIndicators();

        _currentPage = pageName;
        _isPageOpen = true;
        SetLeftPageInteractable(false);

        switch (pageName)
        {
            case "System":
                systemPage.SetActive(true);
                pageIndicatorSistema.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSystemSelected);
                break;

            case "Quest":
                questPage.SetActive(true);
                pageIndicatorQuest.SetActive(true);
                break;

            case "Attributes":
                upgradePage.SetActive(true);
                pageIndicatorAtributos.SetActive(true);
                RefreshUpgradeUI();
                _selectedIndex = 0;
                UpdatePin();
                StartBlinking();
                break;
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed || !_isPageOpen) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        ClosePage();
    }

    public void ClosePage()
    {
        if (_currentPage == "Attributes")
            StopBlinking();

        DeactivateAllPages();
        DeactivateAllIndicators();
        SetLeftPageInteractable(true);

        _isPageOpen = false;
        _currentPage = "";
    }

    private void DeactivateAllPages()
    {
        if (systemPage != null) systemPage.SetActive(false);
        if (questPage != null) questPage.SetActive(false);
        if (upgradePage != null) upgradePage.SetActive(false);
    }

    private void DeactivateAllIndicators()
    {
        if (pageIndicatorSistema != null) pageIndicatorSistema.SetActive(false);
        if (pageIndicatorQuest != null) pageIndicatorQuest.SetActive(false);
        if (pageIndicatorAtributos != null) pageIndicatorAtributos.SetActive(false);
    }

    private void SetLeftPageInteractable(bool interactable)
    {
        float alpha = interactable ? 1f : 150f / 255f;
        SetButtonState(buttonSistema, interactable, alpha);
        SetButtonState(buttonQuest, interactable, alpha);
        SetButtonState(buttonAtributos, interactable, alpha);
    }

    private void SetButtonState(Button button, bool interactable, float alpha)
    {
        if (button == null) return;

        button.interactable = interactable;

        Color c = button.image.color;
        c.a = alpha;
        button.image.color = c;

        var tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            Color tc = tmp.color;
            tc.a = alpha;
            tmp.color = tc;
        }
    }

    //Upgrade: Input

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || _currentPage != "Attributes") return;

        Vector2 direction = context.ReadValue<Vector2>();

        if (direction.x > 0.5f)
        {
            _selectedIndex = Mathf.Min(_selectedIndex + 1, 2);
            UpdatePin();
        }
        else if (direction.x < -0.5f)
        {
            _selectedIndex = Mathf.Max(_selectedIndex - 1, 0);
            UpdatePin();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed || _currentPage != "Attributes") return;

        bool success = _selectedIndex switch
        {
            0 => UpgradeManager.Instance.TryUpgradeElemyTimer(),
            1 => UpgradeManager.Instance.TryUpgradeDamage(),
            2 => UpgradeManager.Instance.TryUpgradeMaxHealth(),
            _ => false
        };

        if (success) RefreshUpgradeUI();

        AudioManager.Instance.PlayEffect(SFXID.SelectArchetype);
    }

    //Upgrade: UI

    private void RefreshUpgradeUI()
    {
        int elemyLevel = LevelToInt(UpgradeManager.Instance.ElemyTimerLevel);
        int damageLevel = LevelToInt(UpgradeManager.Instance.DamageLevel);
        int healthLevel = LevelToInt(UpgradeManager.Instance.MaxHealthLevel);

        SetSlider(elemyTimerSlider, elemyLevel);
        SetSlider(damageSlider, damageLevel);
        SetSlider(maxHealthSlider, healthLevel);

        int insight = InsightSystem.instance.insightPoints;
        insightPointsText.text = insight.ToString();
        noInsightText.gameObject.SetActive(insight < 1);
        timerLevelText.text = LevelToInt(UpgradeManager.Instance.ElemyTimerLevel).ToString();
        damageLevelText.text = LevelToInt(UpgradeManager.Instance.DamageLevel).ToString();
        healthLevelText.text = LevelToInt(UpgradeManager.Instance.MaxHealthLevel).ToString();



        bool hasInsight = insight >= 1;
        SetIconAlpha(elemyTimerIcon, hasInsight || elemyLevel >= MAX_LEVEL);
        SetIconAlpha(damageIcon, hasInsight || damageLevel >= MAX_LEVEL);
        SetIconAlpha(maxHealthIcon, hasInsight || healthLevel >= MAX_LEVEL);
    }

    private void SetSlider(Slider slider, int level)
    {
        if (slider == null) return;
        slider.value = (float)level / MAX_LEVEL;
    }

    private void SetIconAlpha(Image icon, bool full)
    {
        if (icon == null) return;
        Color c = icon.color;
        c.a = full ? iconFullAlpha : iconDimAlpha;
        icon.color = c;
    }

    private int LevelToInt(UpgradeLevel level) => level switch
    {
        UpgradeLevel.None => 0,
        UpgradeLevel.Level1 => 1,
        UpgradeLevel.Level2 => 2,
        UpgradeLevel.level3 => 3,
        UpgradeLevel.level4 => 4,
        UpgradeLevel.Max => 5,
        _ => 0
    };

    // Upgrade: Pins

    private void UpdatePin()
    {
        for (int i = 0; i < _pins.Length; i++)
            _pins[i].SetActive(i == _selectedIndex);

        StopBlinking();
        StartBlinking();
    }

    private void StartBlinking()
    {
        StopBlinking();
        if (_pinImages != null && _selectedIndex < _pinImages.Length && _pinImages[_selectedIndex] != null)
            _blinkCoroutine = StartCoroutine(BlinkPin(_pinImages[_selectedIndex]));
    }

    private void StopBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;

            if (_pinImages != null)
                foreach (var img in _pinImages)
                    if (img != null) SetAlpha(img, blinkMaxAlpha);
        }
    }

    private IEnumerator BlinkPin(Image pinImage)
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.unscaledTime * blinkSpeed) + 1f) / 2f;
            float alpha = Mathf.Lerp(blinkMinAlpha, blinkMaxAlpha, t);
            SetAlpha(pinImage, alpha);
            yield return null;
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}