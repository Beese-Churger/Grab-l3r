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
    public Vector2 checkpointPos;
    public StateType state;
    public bool resetPlayer;

    [SerializeField] private SpriteRenderer respawnBG;
    [SerializeField] private GameObject ExplodePlayer;
    private Color bgColor = new(0, 0, 0, 0);
    private int MaxHealth = 5;
    private int health = 3;
    private int score = 0;
    private int bossLives = 3;
    private int collectables;
    private float respawnTimer = 3f;
    private float respawnTimerValue = 3f;
    private float lastHitTime, hitDelay = 0.3f;
    private MaterialHolder player;
    
    // create game manager instance
    private void Awake()
    {
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
        lastHitTime = Time.time;
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
        //SetGameState(StateType.open);
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
                LevelManager.instance.LoadNextLevel();
                break;
            case StateType.respawn:
                resetPlayer = true;
                checkpointPos = SimpleController.Instance.GetCheckpoint();
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
        if (GameObject.FindWithTag("Player")) // handle damage flash
        {
            if (lastHitTime + hitDelay < Time.time)
            {
                player = GameObject.FindWithTag("Player").GetComponent<MaterialHolder>();
                if (player.matState != 0)
                    player.updateMat(0);
            }
            if (health <= 0)
            {
                GameObject.Find("PlayerToExplode").GetComponent<ExplodeOnAwake>().explode();
                GameObject.FindWithTag("Player").SetActive(false);
            }
        }

        if (health <= 0)
        {   
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
        if (LevelManager.instance.GetCurrentLevel() == "LevelLayout Boss")
            health = bossLives;  
        score = 0;
    }

    public void SetCollectables(int add)
    {
        collectables += add;
    }

    public int GetCollectables()
    {
        return collectables;
    }

    // set player score
    public void SetScore(int add)
    {
        score += add;
    }

    // get current score
    public int GetScore()
    {
        return score;
    }

    // update player health when taking damage
    public void TakeDamage()
    {
        if (lastHitTime + hitDelay < Time.time)
        {
            if (health > 1)
            {
                SimpleController.Instance.damageTaken();
                player.updateMat(1);
            }
            health--;
            lastHitTime = Time.time;
        }
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

    public void GetPlayerHealth(out int MaxHealthRef, out int CurrentHealth)
    {
        MaxHealthRef = MaxHealth;
        CurrentHealth = health;
    }

    // get players health
    public int GetCurrentPlayerHealth()
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
