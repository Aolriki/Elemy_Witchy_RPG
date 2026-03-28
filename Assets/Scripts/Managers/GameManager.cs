using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


[System.Serializable]
public class PlayerData
{
    public TutorialData tutorialData;
    public List<LevelData> levelsData = new List<LevelData>();
    public AudioConfigs audioConfigs = new AudioConfigs();
}

[System.Serializable]
public class TutorialData
{
    //just to show how we can use this class
    //to manage different tutorials
    public bool defaultTutorial;
}

public enum TutorialType
{
    DefaultTutorial
}

public static class GameEvents
{
    public static UnityEvent<string, bool> LoadingLevel = new();
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadPlayerData();
    }

    [HideInInspector] public bool FileLoaded = false;

    [HideInInspector] public UnityEvent FileLoadedEvent = new UnityEvent();

    public PlayerData playerData;

    //Saves player data to a file in StreamingAssets folder
    private string path = Application.streamingAssetsPath + "/save.txt";

    private void OnEnable()
    {
        AddBindings();
    }

    private void OnDisable()
    {
        RemoveBindings();
    }

    private void AddBindings()
    {
        GameEvents.LoadingLevel.AddListener(Loading);
    }

    private void RemoveBindings()
    {
        GameEvents.LoadingLevel.RemoveListener(Loading);
    }

    public void Loading(string LevelName, bool isLoading)
    {
        if(isLoading)
        {
            ScreenManager.Instance.LoadingScreen(true);
            SceneManager.LoadScene(LevelName);
        }
        else
        {
            ScreenManager.Instance.LoadingScreen(false);
            //ScreenManager.Instance.ChangeScreen(Screens.Gameplay);
        }

    }
    
    public void SavePlayerData()
    {
        //in this code, right here, before saving the file, you can get the data
        //from different managers and store them in playerData

        string json = JsonUtility.ToJson(playerData);
        SaveFile(json);
    }

    #region FileManagement

    public void SaveFile(string json)
    {
        File.WriteAllText(path, json);
    }

    private void CreateNewFile()
    {
        playerData = new PlayerData();
    }

    public void LoadPlayerData()
    {
        string fileLoaded = "";

        if (File.Exists(path))
        {
            fileLoaded = File.ReadAllText(path);

            playerData = JsonUtility.FromJson<PlayerData>(fileLoaded);

            SetupData();
        }
        else
        {
            CreateNewFile();
            SavePlayerData();
        }

        FileLoaded = true;
        FileLoadedEvent?.Invoke(); //notify that the file has been loaded to any listener
    }

    void SetupData()
    {
        //after loading the file, here you can send the data
        //to different managers to set them up
    }

    #endregion
}
