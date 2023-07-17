using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    open,           // game opened, load main menu
    end,            // load main menu
    levelChange,    // load next level
    respawn,        // re-load level again when player dies
    boss,           // activate and deactivate boss
    credits         // show credits
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private float bossLives = 10;
    public Vector2 checkpointPos;
    public StateType state;
    public bool resetPlayer;

    [SerializeField] private SpriteRenderer respawnBG;
    private Color bgColor = new(0, 0, 0, 0);
    private float health = 3;
    private int score = 0;
    private float respawnTimer = 3f;
    private float respawnTimerValue = 3f;
    private bool triggeredGameEnd;
    
    [SerializeField] private SpriteRenderer respawnBG;
    [SerializeField] private GameObject ExplodePlayer;
    
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new();
            DontDestroyOnLoad(instance);
        }
        return instance;
    }

    // create game manager instance
    private void Awake()
    {
        //ExplodePlayer = GameObject.Find("PlayerToExplode");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
    }

    // get game manager instance
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new();
            DontDestroyOnLoad(instance);
        }
        return instance;
    }

    private void Start()
    {
        SetGameState(StateType.open);
    }

    // set game state
    public void SetGameState(StateType newState)
    {
        state = newState;
        switch (newState)
        {
            case StateType.end:
                StartCoroutine(LevelManager.instance.LoadLevel("MainMenu"));
                break; ;
            case StateType.open:
                StartCoroutine(LevelManager.instance.LoadLevel("MainMenu"));
                break;
            case StateType.levelChange:
                resetPlayer = true;
                LevelManager.instance.LoadNextLevel();
                break;
            case StateType.respawn:
                ResetGame();
                LevelManager.instance.ReLoadLevel();
                break;
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    // get current game state
    public StateType GetGameState()
    {
        return state;
    }

    private void Update()
    {
        if (health <= 0 || bossLives <= 0)
        {
            if(GameObject.Find("Player"))
            {
                ExplodePlayer.SetActive(true);
                GameObject.Find("Player").SetActive(false);

            }
            
            if (respawnTimer >= 0f)
            {
                respawnTimer -= Time.deltaTime;

                if(respawnTimer < 1.5f)
                {
                    if (bgColor.a < 1)
                    {
                        bgColor.a += Time.deltaTime;
                        respawnBG.color = bgColor;
                    }
                }
            }
            else
            {
                ResetGame();
                SetGameState(StateType.respawn);
                respawnTimer = respawnTimerValue;
            }
        }
    }

    // reset game on respawn
    public void ResetGame()
    {
        health = 3;
        score = 0;
        bossLives = 3;
    }

    // set player score
    public void SetScore(int addToScore){
        score += addToScore;
    }

    // get current score
    public int GetScore()
    {
        return score;
    }

    // update player health when taking damage
    public void TakeDamage()
    {
        health --;
    }

    // set player health to 0
    public void InstantDeath()
    {
        health = 0;
    }

    // upddate boss health
    public void RemoveLife()
    {
        bossLives--;
    }

    // get players health
    public float GetPlayerHP()
    {
        return health;
    }

    // get level managers instance
    public LevelManager GetLevelManager()
    {
        return LevelManager.instance;
    }

    public void FadeIn()
    {
        while (respawnBG.color.a > 0)
        {
            bgColor.a -= Time.deltaTime;
            respawnBG.color = bgColor;
        }
    }
}
