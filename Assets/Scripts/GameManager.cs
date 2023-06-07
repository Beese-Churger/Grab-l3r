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
    public static GameManager instance = null;
    private float health = 90;
    private int score = 0;
    private int enemiesDefeated = 0;
    private bool gamePaused;
    private bool triggeredGameEnd;
    private StateType state;
    public static event Action<StateType> StateChanged;

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
                EndGame();
                break;
            case StateType.start:
                //StartCoroutine(levelManager.LoadLevel("MainMenu"));
                break;
            case StateType.pause:
                break;
            case StateType.levelChange:
                levelManager.LoadNextLevel();
                break;
            case StateType.respawn:
                Respawn();
                break;
            case StateType.boss:
                break;
            case StateType.credits:
                break;
        }
        StateChanged?.Invoke(newState);
    }

    private void Update()
    {
        if (health <= 0)
        {
            // set state to respawn or smt..
        }
    }

    private void EndGame()
    {
        // TODO: what happens when game ends
        // show credits after
        ShowCredits();
        StartCoroutine(levelManager.LoadLevel("Main Menu"));
    }

    public void ResetGame()
    {
        // TODO: reset all variables to initials
        health = 100;
        score = 0;
        triggeredGameEnd = false;
        enemiesDefeated = 0;
    }

    public void Respawn()
    {
        // TODO: load level again
    }

    public void SetScore(int addToScore){
        score += addToScore;
    }

    public int GetScore()
    {
        return score;
    }

    public void ShowCredits()
    {
        // TODO: display credits when the game ends
    }

    public void TakeDamage()
    {
        health -= 30;
        Debug.Log("Health:"+health);
    }
}
