using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level
{
    public string sceneName;

    public Level(string s)
    {
        sceneName = s;
    }
}

public class LevelManager : MonoBehaviour
{
    private Level[] levels;
    private int currentLevelIndex = 0;
    private static LevelManager instance = null;
    private GameManager gameManager;

    // Manually add all levels (scenes) by name to the levels array
    void Start()
    {
        new Level("Environment");
        new Level("Level1");
        new Level("Level2");
        new Level("BossLevel");

        //Debug.Log(levels[currentLevelIndex]);
        // add more levels if there are
    }

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

    // load level after current level is finnished
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            Level level= levels[currentLevelIndex];
            StartCoroutine(LoadLevel(level));
        }
        else
        {
            Debug.Log("All levels loaded");
            //gameManager.SetGameState(StateType.end);

        }

    }

    public void ReLoadLevel()
    {
        Level level = levels[currentLevelIndex];
        StartCoroutine(LoadLevel(level));
    }

    // Load level by index
    private IEnumerator LoadLevel(Level level)
    {
        string scene = level.sceneName;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
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
    }
}
