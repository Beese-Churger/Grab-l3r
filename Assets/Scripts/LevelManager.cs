using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string[] levels = { "MainMenu", "Level1Cutscene", "LevelLayout", "LevelLayout 2", "LevelLayout Boss" };
    public static LevelManager instance = null;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    // load level after current level is finnished
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            StartCoroutine(LoadLevel(currentLevelIndex));
        }
        else
        {
            Debug.Log("All levels loaded");
            // after all levels are finnished, return to main menu
            GameManager.instance.SetGameState(StateType.end);
        }

    }

    public void ReLoadLevel()
    {
        string level = levels[currentLevelIndex];
        StartCoroutine(LoadLevel(level));
    }

    // Load level by index
    private IEnumerator LoadLevel(int index)
    {
        string scene = levels[index];
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        if (EnemyManager.enemyManager != null)
            EnemyManager.enemyManager.AddEnemies();
        AudioManager.Instance.PlayBGMLoop("level1bgm", false);
    }

    // Load level by name
    // Can be used in level selection or for testing purpouses
    public IEnumerator LoadLevel(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (EnemyManager.enemyManager != null)
            EnemyManager.enemyManager.AddEnemies();

        AudioManager.Instance.PlayBGMLoop("level1bgm", false);
    }

    // Get Current level
    public string GetCurrentLevel()
    {
        foreach (string levelName in levels)
        {
            if (SceneManager.GetActiveScene().name == levelName)
            {
                return levelName;
            }
        }
        return "";
    }
}
