using System;
using UnityEngine;


    public enum StateType
    {
        end,            // move to main menu
        start,          // load main menu 
        pause,          // game on pause
        levelChange,    // load next level
        respawn,        // load level again when player dies
        boss,           // activate and deactivate boss
        credits         // show credits
    }

public class GameManager : MonoBehaviour
{
    private float health = 90;
    private int score = 0;
    private int enemiesDefeated = 0;
    private bool gamePaused;
    private bool triggeredGameEnd;

    private StateType state;
    public static event Action<StateType> StateChanged;
    public static GameManager instance = null;

    [SerializeField] private GameObject levelManagerObj = null;
    private LevelManager levelManager = null;

    public static GameManager getInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();
            DontDestroyOnLoad(instance);
        }
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        levelManager = levelManagerObj.GetComponent<LevelManager>();
        // load mainmenu when game is opened
        SetGameState(StateType.start);
    }

    public void SetGameState(StateType newState)
    {
        state=newState;

        switch (newState)
        {
            case StateType.end:
                DisplayCredits();
                StartCoroutine(levelManager.LoadLevel("Main Menu"));
                break;
            case StateType.start:
                StartCoroutine(levelManager.LoadLevel("MainMenu"));
                break;
            case StateType.pause:
                break;
            case StateType.levelChange:
                levelManager.LoadNextLevel();
                break;
            case StateType.respawn:
                ResetGame();
                levelManager.ReLoadLevel();
                break;
            case StateType.boss:
                break;
            case StateType.credits:
                break;
        }
        StateChanged?.Invoke(newState);
    }

    private void DisplayCredits()
    {
       // TODO: credits display
    }

    private void Update()
    {
        if (health <= 0)
        {
            SetGameState(StateType.respawn);
        }
    }

    public void ResetGame()
    {
        // TODO: reset all variables to initials
        health = 100;
        score = 0;
        enemiesDefeated = 0;
        triggeredGameEnd = false;
    }


    public void SetScore(int addToScore){
        score += addToScore;
    }

    public int GetScore()
    {
        return score;
    }

    public void TakeDamage()
    {
        health -= 30;
        Debug.Log("Health:"+health);
    }
}
