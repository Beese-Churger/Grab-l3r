using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //private string[] levels = { "Level1", "Level2", "MainMenu", "Level1Cutscene", "LevelLayout", "LevelLayout 2", "LevelLayout Boss" };
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

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            StartCoroutine(LoadLevel(currentLevelIndex));
        }
        else
        {
            GameManager.instance.SetGameState(StateType.end);
        }
    }

    public void ReLoadLevel()
    {
        CheckCurrentIndex();
        string level = levels[currentLevelIndex];
        StartCoroutine(LoadLevel(level));
    }

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

    public IEnumerator LoadLevel(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (EnemyManager.enemyManager != null)
            EnemyManager.enemyManager.AddEnemies();

        //AudioManager.Instance.PlayBGMLoop("level1bgm", false);
    }

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
    public void SetCurrentLevelIndex(int idx)
    {
        currentLevelIndex = idx;
    }
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    private void CheckCurrentIndex()
    {
        string currentLevel = GetCurrentLevel();
        for (int i = 0; i < levels.Length; ++i)
        {
            if (currentLevel == levels[i])
            {
                currentLevelIndex = i;
                return;
            }
        }
    }
}
