using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UpgradeMenuUI : MonoBehaviour
{
    // Painel
    [Header("Painel")]
    [SerializeField] private GameObject upgradePanel;

    // Player Input
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    // Sliders Circulares
    [Header("Circular Sliders")]
    [SerializeField] private Slider elemyTimerSlider;
    [SerializeField] private Slider damageSlider;
    [SerializeField] private Slider maxHealthSlider;

    // Ícones dos Upgrades (para feedback de Insight insuficiente)
    [Header("Upgrade Icons")]
    [SerializeField] private Image elemyTimerIcon;
    [SerializeField] private Image damageIcon;
    [SerializeField] private Image maxHealthIcon;

    // Pinos
    [Header("Pinos")]
    [SerializeField] private GameObject pin_ElemyTimer;
    [SerializeField] private GameObject pin_Damage;
    [SerializeField] private GameObject pin_MaxHealth;

    // UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI insightPointsText;

    // Configurações de Piscar
    [Header("Pin Blink Settings")]
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float blinkMinAlpha = 0.2f;
    [SerializeField] private float blinkMaxAlpha = 1f;

    // Configurações de Ícone com Insight Baixo
    [Header("Icon Dim Settings")]
    [SerializeField] private float iconDimAlpha = 150f / 255f;
    [SerializeField] private float iconFullAlpha = 1f;

    private const int MAX_LEVEL = 5;

    // Estado interno
    private int _selectedIndex = 0;
    private bool _isPanelOpen = false;
    private Coroutine _blinkCoroutine;

    // Pinos como array para facilitar acesso
    private GameObject[] _pins;
    private Image[] _pinImages;

    // Inicialização
    private void Start()
    {
        _pins = new GameObject[] { pin_ElemyTimer, pin_Damage, pin_MaxHealth };
        _pinImages = new Image[]
        {
            pin_ElemyTimer.GetComponent<Image>(),
            pin_Damage.GetComponent<Image>(),
            pin_MaxHealth.GetComponent<Image>()
        };

        upgradePanel.SetActive(false);
        InsightSystem.instance.onInsightChange += RefreshUI;
        RefreshUI();
        UpdatePin();
    }

    private void OnDisable()
    {
        if (InsightSystem.instance != null)
            InsightSystem.instance.onInsightChange -= RefreshUI;

        StopBlinking();
    }

    // Input Actions
    public void OnOpenUpgradeMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TogglePanel();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || !_isPanelOpen) return;

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
        if (!context.performed || !_isPanelOpen) return;

        bool success = _selectedIndex switch
        {
            0 => UpgradeManager.Instance.TryUpgradeElemyTimer(),
            1 => UpgradeManager.Instance.TryUpgradeDamage(),
            2 => UpgradeManager.Instance.TryUpgradeMaxHealth(),
            _ => false
        };

        if (success) RefreshUI();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed || !_isPanelOpen) return;
        TogglePanel();
    }

    // Abrir/Fechar painel
    private void TogglePanel()
    {
        _isPanelOpen = !_isPanelOpen;
        upgradePanel.SetActive(_isPanelOpen);

        if (_isPanelOpen)
        {
            playerInput.SwitchCurrentActionMap("Ui");
            RefreshUI();
            StartBlinking();
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            StopBlinking();
        }
    }

    // Atualiza sliders, ícones e texto
    private void RefreshUI()
    {
        int elemyLevel = LevelToInt(UpgradeManager.Instance.ElemyTimerLevel);
        int damageLevel = LevelToInt(UpgradeManager.Instance.DamageLevel);
        int healthLevel = LevelToInt(UpgradeManager.Instance.MaxHealthLevel);

        SetSlider(elemyTimerSlider, elemyLevel);
        SetSlider(damageSlider, damageLevel);
        SetSlider(maxHealthSlider, healthLevel);

        int insight = InsightSystem.instance.insightPoints;
        insightPointsText.text = insight.ToString();

        // Escurece ícones se não houver Insight suficiente
        bool hasInsight = insight >= 1;
        SetIconAlpha(elemyTimerIcon, hasInsight || elemyLevel >= MAX_LEVEL);
        SetIconAlpha(damageIcon, hasInsight || damageLevel >= MAX_LEVEL);
        SetIconAlpha(maxHealthIcon, hasInsight || healthLevel >= MAX_LEVEL);
    }

    // Slider: valor normalizado entre 0 e 1
    private void SetSlider(Slider slider, int level)
    {
        if (slider == null) return;
        slider.value = (float)level / MAX_LEVEL;
    }

    // Alpha nos ícones
    private void SetIconAlpha(Image icon, bool full)
    {
        if (icon == null) return;
        Color c = icon.color;
        c.a = full ? iconFullAlpha : iconDimAlpha;
        icon.color = c;
    }

    // Converte enum para int
    private int LevelToInt(UpgradeLevel level) => level switch
    {
        UpgradeLevel.None => 0,
        UpgradeLevel.Level1 => 1,
        UpgradeLevel.Level2 => 2,
        UpgradeLevel.Max => 3,
        _ => 0
    };

    // Pino: ativa o correto e reinicia blink
    private void UpdatePin()
    {
        for (int i = 0; i < _pins.Length; i++)
            _pins[i].SetActive(i == _selectedIndex);

        if (_isPanelOpen)
        {
            StopBlinking();
            StartBlinking();
        }
    }

    // Efeito de piscar no pino ativo
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

            // Restaura alpha de todos os pinos
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