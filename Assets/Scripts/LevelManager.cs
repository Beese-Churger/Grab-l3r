using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string[] levels = { "Level1Cutscene", "LevelLayout", "LevelLayout 2", "LevelLayout Boss" };
    private string[] levelsBGM = { "", "level1bgm", "level1bgm", "level2bgm", "bossbgm" };
    public static LevelManager instance = null;

    private int currentLevelIndex = 0;

    // create an instance of level manager
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            CheckCurrentIndex();
            DontDestroyOnLoad(instance);
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    // load next level by level index
    public void LoadNextLevel()
    {
        currentLevelIndex++;
        // check if all the levels are loaded
        if (currentLevelIndex < levels.Length)
        {
            StartCoroutine(LoadLevel(currentLevelIndex));
        }
        else
        {
            GameManager.instance.SetGameState(StateType.end);
        }
    }

    // reload level on respawn 
    public void ReLoadLevel()
    {
        string level = levels[currentLevelIndex];
        StartCoroutine(LoadLevel(level));
        GameManager.instance.FadeIn();
    }

    // load level by index
    private IEnumerator LoadLevel(int index)
    {
        string scene = levels[index];
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        if (EnemyManager.enemyManager != null)
        {
            EnemyManager.enemyManager.AddEnemies();
        }
           
        PlayLevelBGM();
    }

    // load level by name
    public IEnumerator LoadLevel(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (EnemyManager.enemyManager != null)
        {
            EnemyManager.enemyManager.AddEnemies();
        }
        
        PlayLevelBGM();
    }

    // return current level name
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

    // set level index
    public void SetCurrentLevelIndex(int idx)
    {
        currentLevelIndex = idx;
    }

    // get current level index
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    // check index
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

    // Check which level the player is in before playing the bgm
    private void PlayLevelBGM()
    {
        AudioManager.Instance.PlayBGMLoop(levelsBGM[currentLevelIndex], false);
    }
}
