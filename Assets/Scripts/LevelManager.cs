using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string[] levels = { "MainMenu", "Environment", "Level1", "Level2", "BossLevel", "LevelLayout Boss" };
    private static LevelManager instance = null;
    private GameManager gameManager;
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
            // after all levels are finnished, return to main menu
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
