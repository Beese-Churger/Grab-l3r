using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

//public class Level
//{
//    public string sceneName;

//    public Level(string s)
//    {
//        sceneName = s;
//    }
//}

[System.Serializable]
public class LevelManager : MonoBehaviour
{
    private string[] levels = { "MainMenu", "Environment", "Level1", "Level2", "BossLevel" };
    private int currentLevelIndex = 0;
    private static LevelManager instance = null;
    private GameManager gameManager;

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

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            currentLevelIndex = 0;
            gameManager.SetGameState(StateType.end);
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
