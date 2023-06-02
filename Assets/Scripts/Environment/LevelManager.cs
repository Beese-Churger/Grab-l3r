using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Manually add all levels (scenes) by name to the levels array
    void Start()
    {
        new Level("Environment");
        new Level("Jontest");
        // add more levels if there are
    }

    public class Level
    {
        public string sceneName;

        public Level(string s)
        {
            sceneName = s;
        }
    }

    private Level[] levels;
    private int currentLevelIndex = 0;
    private static LevelManager instance;

    public static LevelManager Instance()
    {
        if(instance==null)
        {
            instance= new LevelManager();
        }
        return instance;
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
        }

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
