using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int highscore;

    [SerializeField] private SpriteRenderer respawnBG;
    [SerializeField] private GameObject ExplodePlayer;
    [SerializeField] private GameObject ExplodePrefab;
    private Color bgColor = new(0, 0, 0, 0);
    private int MaxHealth = 5;
    private int health = 3;
    private int score = 0;
    private int bossLives = 5;
    private int collectables;
    private float respawnTimer = 3f;
    private float respawnTimerValue = 3f;

    private float endTimer = 5f;
    private float endTimerValue = 5f;

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
        score = 0;
        highscore = PlayerPrefs.GetInt("highscore", highscore);
        //SetGameState(StateType.open);
    }

    // set game state
    public void SetGameState(StateType newState)
    {
        state = newState;
        switch (newState)
        {
            case StateType.end:
                LevelManager.onLoaderCallback = () => StartCoroutine(LevelManager.instance.LoadLevel("MainMenu"));
                SceneManager.LoadScene("LoadingScene");
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
                GameObject player = GameObject.FindWithTag("Player");
                GameObject.Find("PlayerToExplode").GetComponent<ExplodeOnAwake>().explode(player);
                AudioManager.Instance.PlaySFX("player_death" + Random.Range(1, 5), player.transform.position);
                player.SetActive(false);
                GameObject explosion = Instantiate(ExplodePrefab, player.transform.position, Quaternion.identity);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
                AudioManager.Instance.PlaySFX("explode", player.transform.position);
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

        if(score>highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
            PlayerPrefs.Save();
        }

        if (Boss.instance)
            if (!Boss.instance.gameObject.activeInHierarchy)
                End();
 
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
    public void End()
    {

        if (endTimer >= 0f)
        {
            endTimer -= Time.deltaTime;

            if (endTimer < 1.5f)
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
            endTimer = endTimerValue;
            Debug.Log("This");
            LevelManager.instance.LoadNextLevel();
            FadeIn();
        }
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
