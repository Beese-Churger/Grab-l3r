using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public class LevelData
    {
        public string sceneName;

        public LevelData(string scene)
        {
            sceneName= scene;
        }
    }

    private LevelData[] levels;
    private int currentLevel = 0;
    public bool changeLevel = false;

    // Load level by name
    public void LoadLevel(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    // load level after current level is finnished
    public void LoadNextLevel(LevelData levels)
    {

    }

    // Manually add all levels (scenes) by name to the levels array
    void Start()
    {
        new LevelData("Environment");
        new LevelData("Jontest");
        // add more levels if there are
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
}
