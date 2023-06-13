using System;
using Unity.VisualScripting;
using UnityEngine;


    public enum StateType
    {
        open,           // game opened, load main menu
        end,            // move to main menu     
        start,          // load first level
        levelChange,    // load next level
        respawn,        // load level again when player dies
        boss,           // activate and deactivate boss
        credits         // show credits
    }

public class GameManager : MonoBehaviour
{
    private float health = 90;
    private int score = 0;
    //private int enemiesDefeated = 0;
    private bool gamePaused;
    //private bool triggeredGameEnd;

    private StateType state;
    //public static event Action<StateType> StateChanged;
    public static GameManager instance = null;
    private LevelManager levelManager;

    public static GameManager GetInstance()
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
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    public void SetGameState(StateType newState)
    {
        state = newState;

        switch (newState)
        {
            case StateType.end:
                //DisplayCredits();
                //StartCoroutine(levelManager.LoadLevel("MainMenu"));
                break; ;
            case StateType.start:
                levelManager.LoadNextLevel();
                break;
            case StateType.open:
                StartCoroutine(levelManager.LoadLevel("MainMenu"));
                break;
            case StateType.levelChange:
                levelManager.LoadNextLevel();
                break;
            case StateType.respawn:
                ResetGame();
                levelManager.ReLoadLevel();
                break;
        }
        //StateChanged?.Invoke(newState);
    }

    private void DisplayCredits()
    {
       // TODO: credits display
    }

    private void Update()
    {
        if (health <= 0)
        {
            ResetGame();
            SetGameState(StateType.respawn);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SetGameState(StateType.levelChange);
        }
    }

    public void ResetGame()
    {
        // TODO: reset all variables to initials
        health = 100;
        score = 0;
        //enemiesDefeated = 0;
        //triggeredGameEnd = false;
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
