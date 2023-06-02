using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            LoadLevel(level);
        }
        else
        {
            Debug.Log("All levels loaded");
        }

    }

    // Load level by index
    public void LoadLevel(Level level)
    {
        string scene = level.sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    // Load level by name
    // Can be used in level selection or for testing purpouses
    public void LoadLevel(string scene)
    { 
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
