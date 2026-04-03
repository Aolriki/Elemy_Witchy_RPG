using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Screens
{
    None,
    PressAnyButton,
    MainMenu,
    SelectFile,
    ArchetypeSelection,
    Gameplay,
    Pause,
    GameOver,
}

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitInputActions();
    }

    public Screens currentScreen = Screens.MainMenu;

    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mainMenuFirstSelected;

    [Header("SelectFile")]
    [SerializeField] private GameObject selectFilePanel;

    [Header("ArchetypeSelection")]
    [SerializeField] private GameObject archetypeSelectionPanel;

    [Header("Gameplay")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private HealthBar healthBar;

    [Header("LoadingScreen")]
    public GameObject loadingPanel;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Configs")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("BlackScreen")]
    [SerializeField] private CanvasGroup blackScreenPanel;

    [Header("Inputs")]
    private PlayerInputActions playerControls;
    private InputAction pauseAction;

    private void Start()
    {
        Debug.Log("Start chamado!");
        ChangeScreen(Screens.MainMenu);
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (currentScreen == Screens.Gameplay)
                ChangeScreen(Screens.Pause);
            else if (currentScreen == Screens.Pause)
                ChangeScreen(Screens.Gameplay);
        }
    }

    #region Inputs

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs();
    }

    private void InitInputActions()
    {
        playerControls = new PlayerInputActions();
    }

    public void EnableInputs()
    {
        pauseAction = playerControls.UI.Pause;
        pauseAction?.Enable();
    }

    public void DisableInputs()
    {
        pauseAction?.Disable();
    }

    private void DisablePlayerControls()
    {
        if (PlayerController.instance != null)
            PlayerController.instance.DisableInputs();
    }

    private void EnablePlayerControls()
    {
        if (PlayerController.instance != null)
            PlayerController.instance.EnableInputs();
    }

    #endregion

    public void ChangeScreenByString(string screenName)
    {
        if (Enum.TryParse(screenName, out Screens parsedScreen))
        {
            ChangeScreen(parsedScreen);
        }
        else
        {
            Debug.LogWarning($"'{screenName}' is not a valid name for a Screen.");
        }
    }

    public void ChangeScreen(Screens screen)
    {
        Debug.Log("ChangeScreen chamado: " + screen);
        Screens lastScreen = currentScreen;
        currentScreen = screen;

        switch (currentScreen)
        {
            case Screens.MainMenu:
                DesactivateAllScreens();
                mainMenuPanel.SetActive(true);
                DisablePlayerControls();

                if (mainMenuFirstSelected != null)
                    EventSystem.current?.SetSelectedGameObject(mainMenuFirstSelected);

                break;

            case Screens.SelectFile:
                DesactivateAllScreens();
                selectFilePanel.SetActive(true);
                break;

            case Screens.ArchetypeSelection:
                DesactivateAllScreens();
                archetypeSelectionPanel.SetActive(true);
                PlayerController.instance?.playerInput.SwitchCurrentActionMap("UI");
                break;

            case Screens.Gameplay:
                DesactivateAllScreens();

                if (!ArchetypeManager.Instance.HasSelectedArchetype())
                {
                    archetypeSelectionPanel.SetActive(true);
                    PlayerController.instance?.playerInput.SwitchCurrentActionMap("UI");
                    return;
                }

                hudPanel.SetActive(true);
                EnablePlayerControls();
                PlayerController.instance?.playerInput.SwitchCurrentActionMap("Player");

                if (lastScreen == Screens.Pause)
                {
                    Time.timeScale = 1f;
                    AudioManager.Instance.ResumeMusic();
                }

                break;

            case Screens.Pause:
                DesactivateAllScreens();
                pausePanel.SetActive(true);
                Time.timeScale = 0f;
                PlayerController.instance?.playerInput.SwitchCurrentActionMap("UI");
                AudioManager.Instance.PauseMusic();
                break;

            case Screens.GameOver:
                DesactivateAllScreens();
                gameOverPanel.SetActive(true);
                break;
        }
    }

    public void DesactivateAllScreens()
    {
        mainMenuPanel.SetActive(false);
        selectFilePanel.SetActive(false);
        archetypeSelectionPanel.SetActive(false);
        hudPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void LoadingScreen(bool activate)
    {
        loadingPanel.SetActive(activate);
    }

    public void StartGame()
    {
        Debug.Log("StartGame chamado!");
        BlackScreen();
        StartCoroutine(StartGameDelayed());
    }

    private IEnumerator StartGameDelayed()
    {
        yield return new WaitForSecondsRealtime(2f);

        // Força o HUD sem verificar arquétipo ainda
        DesactivateAllScreens();
        hudPanel.SetActive(true);
        EnablePlayerControls();
        currentScreen = Screens.Gameplay; // agora é seguro, OnCancel tem guarda

        yield return new WaitForSecondsRealtime(0.5f);

        ChangeScreen(Screens.ArchetypeSelection);
    }

    public void QuitGame()
    {
        BlackScreen();
        StartCoroutine(QuitDelayed());
    }

    private IEnumerator QuitDelayed()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerController.instance?.RemoveInputBindings();
        BlackScreen();
        StartCoroutine(ReturnToMainMenuDelayed());
    }

    private IEnumerator ReturnToMainMenuDelayed()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BlackScreen()
    {
        StartCoroutine(BlackScreenCoroutine());
    }

    private IEnumerator BlackScreenCoroutine()
    {
        blackScreenPanel.gameObject.SetActive(true);
        blackScreenPanel.DOFade(1f, 1f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(2f);
        blackScreenPanel.DOFade(0, 1f).SetUpdate(true).OnComplete(() =>
        {
            blackScreenPanel.gameObject.SetActive(false);
        });
    }

    public void SetSoundConfigs()
    {
        float musicVolume = musicSlider.value;
        AudioManager.Instance.audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);

        float sfxVolume = sfxSlider.value;
        AudioManager.Instance.audioMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
    }

    public void SaveSoundConfigs()
    {
        GameManager.Instance.playerData.audioConfigs.musicVolume = musicSlider.value;
        GameManager.Instance.playerData.audioConfigs.sfxVolume = sfxSlider.value;
        GameManager.Instance.SavePlayerData();
    }

    public void ShowTutorial(TutorialType tutorialType)
    {
        Time.timeScale = 0f;
    }

    public void ExitTutorial()
    {
        DisableAllTutorials();
        Time.timeScale = 1f;
    }

    public void DisableAllTutorials()
    {
        //
    }

    public void PlayGenericButton()
    {
        //AudioManager.Instance.PlayEffect("GenericButton");
    }

    public void PlayCloseButton()
    {
        //AudioManager.Instance.PlayEffect("CloseButton");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadSceneByString(string sceneName)
    {
        GameEvents.LoadingLevel?.Invoke(sceneName, true);
    }

    #region HealthBar
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    #endregion
}