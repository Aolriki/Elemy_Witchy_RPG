using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArchetypeSelectionUI : MonoBehaviour
{
    [Header("Painel")]
    [SerializeField] private GameObject selectionPanel;

    [Header("Arquétipos")]
    [SerializeField] private ArchetypeData archetypeCautelosa;
    [SerializeField] private ArchetypeData archetypeExemplar;
    [SerializeField] private ArchetypeData archetypeProativa;

    [Header("Sliders de Confirmação")]
    [SerializeField] private Slider sliderCautelosa;
    [SerializeField] private Slider sliderExemplar;
    [SerializeField] private Slider sliderProativa;

    [Header("Selection Indicators")]
    [SerializeField] private GameObject indicatorCautelosa;
    [SerializeField] private GameObject indicatorExemplar;
    [SerializeField] private GameObject indicatorProativa;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI archetypeNameText;
    [SerializeField] private TextMeshProUGUI archetypeDescriptionText;

    [Header("Valores")]
    [SerializeField] private TextMeshProUGUI timerBaseStatusText;
    [SerializeField] private TextMeshProUGUI damageBaseStatusText;
    [SerializeField] private TextMeshProUGUI healthBaseStatusText;

    [Header("Ilustração")]
    [SerializeField] private Image archetypeIllustration;

    [Header("Configurações de Blink")]
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float blinkMinAlpha = 0.2f;
    [SerializeField] private float blinkMaxAlpha = 1f;

    [Header("Configurações de Fill")]
    [SerializeField] private float fillSpeed = 1f;

    [Header("Foco Inicial")]
    [SerializeField] private GameObject firstSelected;

    private ArchetypeData[] _archetypes;
    private Slider[] _sliders;
    private GameObject[] _indicators;
    private Image[] _indicatorImages;

    private int _selectedIndex = 0;
    private bool _isHolding = false;
    private Coroutine _blinkCoroutine;


    private void Awake()
    {
        _archetypes = new ArchetypeData[]
        {
            archetypeCautelosa,
            archetypeExemplar,
            archetypeProativa
        };

        _sliders = new Slider[]
        {
            sliderCautelosa,
            sliderExemplar,
            sliderProativa
        };

        _indicators = new GameObject[]
        {
            indicatorCautelosa,
            indicatorExemplar,
            indicatorProativa
        };

        _indicatorImages = new Image[]
        {
            indicatorCautelosa.GetComponent<Image>(),
            indicatorExemplar.GetComponent<Image>(),
            indicatorProativa.GetComponent<Image>()
        };
    }

    private void OnEnable()
    {
        if (_sliders == null) return;

        PlayerController.instance?.playerInput.SwitchCurrentActionMap("UI");

        _selectedIndex = 0;
        _isHolding = false;

        foreach (var s in _sliders)
            s.value = 0f;

        UpdateVisuals();
        StartBlinking();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void OnDisable()
    {
        StopBlinking();
    }

    private void Update()
    {
        HandleSliderFill();
    }


    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed || _isHolding) return;
        if (!selectionPanel.activeSelf) return; // <- guard

        Vector2 dir = context.ReadValue<Vector2>();

        if (dir.x > 0.5f)
            ChangeSelection(1);
        else if (dir.x < -0.5f)
            ChangeSelection(-1);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!selectionPanel.activeSelf) return; // <- guard

        if (context.performed)
            _isHolding = true;
        else if (context.canceled)
            _isHolding = false;
    }

    private void ChangeSelection(int delta)
    {
        _sliders[_selectedIndex].value = 0f;
        _selectedIndex = Mathf.Clamp(_selectedIndex + delta, 0, _archetypes.Length - 1);

        UpdateVisuals();
        StopBlinking();
        StartBlinking();
    }

    private void UpdateVisuals()
    {
        ArchetypeData current = _archetypes[_selectedIndex];

        archetypeNameText.text = current.archetypeName;
        archetypeDescriptionText.text = current.description;
        archetypeIllustration.sprite = current.illustration;

        timerBaseStatusText.text = current.elemyTimer.ToString("F0"); 
        damageBaseStatusText.text = current.damage.ToString();         
        healthBaseStatusText.text = current.maxHealth.ToString("F0"); 


        for (int i = 0; i < _indicators.Length; i++)
            _indicators[i].SetActive(i == _selectedIndex);
    }

 

    private void HandleSliderFill()
    {
        if (_sliders == null) return;

        Slider current = _sliders[_selectedIndex];

        if (_isHolding)
        {
            current.value += Time.deltaTime / fillSpeed;

            if (current.value >= 1f)
            {
                current.value = 1f;
                ConfirmSelection();
            }
        }
        else
        {
            current.value = Mathf.Max(current.value - Time.deltaTime / fillSpeed, 0f);
        }
    }

    private void ConfirmSelection()
    {
        _isHolding = false;
        StopBlinking();

        switch (_selectedIndex)
        {
            case 0: ArchetypeManager.Instance.SelectCautelosa(); break;
            case 1: ArchetypeManager.Instance.SelectExemplar(); break;
            case 2: ArchetypeManager.Instance.SelectProativa(); break;
        }

        PlayerController.instance?.playerInput.SwitchCurrentActionMap("Player");
        selectionPanel.SetActive(false);

        AudioManager.Instance.PlayEffect(SFXID.SelectArchetype);
    }


    private void StartBlinking()
    {
        StopBlinking();
        if (_indicatorImages != null &&
            _selectedIndex < _indicatorImages.Length &&
            _indicatorImages[_selectedIndex] != null)
        {
            _blinkCoroutine = StartCoroutine(BlinkIndicator(_indicatorImages[_selectedIndex]));
        }
    }

    private void StopBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;

            if (_indicatorImages != null)
                foreach (var img in _indicatorImages)
                    if (img != null) SetAlpha(img, blinkMaxAlpha);
        }
    }

    private IEnumerator BlinkIndicator(Image img)
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.unscaledTime * blinkSpeed) + 1f) / 2f;
            float alpha = Mathf.Lerp(blinkMinAlpha, blinkMaxAlpha, t);
            SetAlpha(img, alpha);
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