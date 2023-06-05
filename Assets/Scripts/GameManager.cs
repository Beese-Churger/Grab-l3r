using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Android;
using static Terrain;


    public enum StateType
    {
        end,            // move to main menu
        start,          // load main menu 
        pause,          // game on pause
        levelPassed,    // load next level
        respawn,        // load level again when player dies
        boss,           // activate and deactivate boss
        credits         // show credits
    }

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int health = 100;
    public int score = 0;
    public int enemiesDefeated = 0;
    private bool gameStarted;
    public bool gamePaused;
    public bool triggeredGameEnd;
    [SerializeField] StateType states;

    public static GameManager getInstance()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(instance);
            instance = new GameManager();
        }
        return instance;
    }

    private void Awake()
    {
        instance= this;
    }

    public void SetGameEvent(StateType newState)
    {
        this.states= newState;
        // TODO: hanle event change
    }

    private void Update()
    {
        // TODO: fill with event associated functions etc...
        switch(states)
            {
                case StateType.end:
                    ShowCredits();
                    break;
                case StateType.start:
                    break;
                case StateType.pause:
                    break;
                case StateType.levelPassed:
                    break;
                case StateType.respawn:
                    Respawn();
                    break;
                case StateType.boss:
                    break;
                case StateType.credits:
                    break;
        }
    }

    public void ResetGame()
    {
        // TODO: reset all variables to initials
        this.health = 100;
        this.score = 0;
    }

    public void Respawn()
    {
        // TODO: load level
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

    }

}
