using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public struct Level
    {
        private int level;
        private bool isCompleted;

        public Level(int levelno, bool completed)
        {
            level = levelno;
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
    private string[] levels = { "MainMenu", "Level1Cutscene", "level_forestTutorial", "LevelLayout 2", "LevelLayout Boss" };
    private string[] levelsBGM = { "mainmenubgm", "level1bgm", "level1bgm", "level2bgm", "bossbgm" };
    public List<Level> arrLevels;
    public static LevelManager instance = null;

    private int currentLevelIndex = 0;

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
            CheckCurrentIndex();
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
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    // load next level by level index
    public void LoadNextLevel()
    {
        if (currentLevelIndex != -1)
        {
           // Debug.Log(currentLevelIndex);
            arrLevels[currentLevelIndex] = new Level(currentLevelIndex, true);
           // Debug.Log(arrLevels.Count);
        }
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (currentLevelIndex > 1)
        {
            if (EnemyManager.enemyManager != null)
                EnemyManager.enemyManager.AddEnemies();
            PlayLevelBGM(false);
        }

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
        //CheckCurrentIndex();
        return currentLevelIndex;
    }

    // Get level index for a specific scene
    public int GetLevelIndexWithName(string name)
    {
        for (int i = 0; i < levels.Length; ++i)
        {
            if (name == levels[i])
            {
                //Debug.Log(i);
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
                return;
            }
        }
    }

    // Check which level the player is in before playing the bgm
    public void PlayLevelBGM(bool loop)
    {
       CheckCurrentIndex();
       AudioManager.Instance.PlayBGMLoop(levelsBGM[currentLevelIndex], loop);
    }

}
