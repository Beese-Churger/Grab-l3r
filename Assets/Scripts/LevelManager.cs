using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
    private static Action onLoaderCallback;
    private string[] levels = { "MainMenu", "Level1Cutscene", "Level_forestTutorial", "LevelLayout 2", "LevelLayout Boss", "EndingCutscene" };
    private string[] levelsBGM = { "mainmenubgm", "introbgm", "level1bgm", "level2bgm", "bossbgm", "endbgm" };
    public List<Level> arrLevels;
    public static LevelManager instance = null;

    private int currentLevelIndex = 0;

    public struct Level
    {
        private int level;
        private bool isCompleted;

        public Level(int levelNo, bool completed)
        {
            level = levelNo;
            isCompleted = completed;
        }

        public int GetIndex()
        {
            return level;
        }

        public bool Completed()
        {
            return isCompleted;
        }
    }

    // create an instance of level manager
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    private void Start()
    {
        arrLevels = new List<Level> {
            new(0,false),
            new(1,false),
            new(2,false),
            new(3,false),
            new(4,false)
        };
        CheckCurrentIndex();
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    // load next level by level index
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        // If player has reached the level then it will be unlocked for them in the load game menu
        if (currentLevelIndex != -1)
        {
            arrLevels[currentLevelIndex] = new Level(currentLevelIndex, true);
        }        
        // check if all the levels are loaded
        if (currentLevelIndex < levels.Length)
        {
            onLoaderCallback = () => StartCoroutine(LoadLevel(currentLevelIndex));
            SceneManager.LoadScene("LoadingScene");
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
        currentLevelIndex = index;
        string scene = levels[index];
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        if (EnemyManager.enemyManager != null)
        {
            EnemyManager.enemyManager.AddEnemies();
        }
        
        PlayLevelBGM(false);
    }

    // load level by name
    public IEnumerator LoadLevel(string scene)
    {
        CheckTargetedLevel(scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (EnemyManager.enemyManager != null)
            EnemyManager.enemyManager.AddEnemies();
        //PlayLevelBGM(false);
    }

    // return current level name
    public string GetCurrentLevel()
    {
        foreach (string levelName in levels)
        {
            if (SceneManager.GetActiveScene().name.ToUpper() == levelName.ToUpper())
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

    // Get level index for a specific scene
    public int GetLevelIndexWithName(string name)
    {
        for (int i = 0; i < levels.Length; ++i)
        {
            if (name == levels[i])
            {
                return i;
            }
        }
        return -1;
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
                Debug.Log(currentLevelIndex);
                return;
            }
        }
    }
    private void CheckTargetedLevel(string target)
    {
        for (int i = 0; i < levels.Length; ++i)
        {
            if (target == levels[i])
            {
                currentLevelIndex = i;
                Debug.Log(currentLevelIndex);
                return;
            }
        }
    }
    // Check which level the player is in before playing the bgm
    public void PlayLevelBGM(bool loop)
    {
       AudioManager.Instance.PlayBGMLoop(levelsBGM[currentLevelIndex], loop);
    }
    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
