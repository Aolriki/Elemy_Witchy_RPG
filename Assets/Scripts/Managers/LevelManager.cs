using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public string levelName;

    public Transform playerSpawn;

    public Transform lastSafePlace;

    public float timeToLoadScene = 2f;

    protected virtual void Start()
    {
        StartCoroutine(LoadLevel());
    }

    protected virtual IEnumerator LoadLevel()
    {
        while (!GameManager.Instance.FileLoaded) yield return null;

        LevelData targetLevelData = null;

        foreach (LevelData levelData in GameManager.Instance.playerData.levelsData)
        {
            if (levelData.levelName == levelName)
            {
                targetLevelData = levelData;
                break;
            }
        }

        if (targetLevelData == null) Debug.Log("No saved data for this world");
        else
        {
            //here we can manage the loading of the level based on the saved data
        }

        yield return new WaitForSeconds(timeToLoadScene);
        PlayerController.instance.currentControlledBeing = Player.instance;
        PlayerController.instance.Init();
        Player.instance.transform.position = playerSpawn.position;

        GameEvents.LoadingLevel?.Invoke(levelName, false);
    }

    public void SaveLevel()
    {
        LevelData targetLevelData = null;

        if (GameManager.Instance.playerData.levelsData.Count > 0)
        {
            foreach (LevelData levelData in GameManager.Instance.playerData.levelsData)
            {
                if (levelData.levelName == levelName)
                {
                    targetLevelData = levelData;
                    break;
                }
            }
        }

        if (targetLevelData == null)
        {
            targetLevelData = new LevelData { levelName = levelName };
            GameManager.Instance.playerData.levelsData.Add(targetLevelData);
        }

        //here we can manage the saving of the level, getting the informations from the level
        //and saving them into targetLevelData
    }

    public IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(0.4f);

        ScreenManager.Instance.BlackScreen();

        yield return new WaitForSeconds(1.2f);

        if (lastSafePlace == null)
            Player.instance.transform.position = playerSpawn.position;
        else
            Player.instance.transform.position = lastSafePlace.position;

        Player.instance.health.ResetLife();

        Player.instance.Set(Player.instance.fallState);

    }

}
