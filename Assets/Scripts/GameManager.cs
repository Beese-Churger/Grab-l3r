using UnityEngine;

public enum StateType
{
    open,           // game opened, load main menu
    end,            // move to main menu
    levelChange,    // load next level
    respawn,        // load level again when player dies
    boss,           // activate and deactivate boss
    credits         // show credits
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer respawnBG;
    private Color bgColor = new(0, 0, 0, 0);
    public static GameManager instance = null;
    private float bossLives = 3;
    public Vector2 checkpointPos;
    private float health = 90;
    private int score = 0;
    private bool triggeredGameEnd;
    public StateType state;
    private float respawnTimer = 3f, respawnTimerValue = 3f;

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new();
            DontDestroyOnLoad(instance);
        }
        return instance;
    }

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
    }

    private void Start()
    {
        //SetGameState(StateType.open);
    }

    public void SetGameState(StateType newState)
    {
        state = newState;
        //OnStateChange();

        switch (newState)
        {
            case StateType.end:
                //DisplayCredits();
                StartCoroutine(LevelManager.instance.LoadLevel("MainMenu"));
                break; ;
            case StateType.open:
                StartCoroutine(LevelManager.instance.LoadLevel("MainMenu"));
                break;
            case StateType.levelChange:
                LevelManager.instance.LoadNextLevel();
                var player = GameObject.Find("Player").GetComponent<TestPlayer>();
                player.ResetPlayer();
                //checkpointPos = player.transform.position;
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

    public StateType GetGameState()
    {
        return this.state;
    }

    private void DisplayCredits()
    {
       // TODO: credits display
    }

    private void Update()
    {
        if (health <= 0 || bossLives <= 0)
        {
            if (bgColor.a < 1)
            {
                bgColor.a += Time.deltaTime;
                respawnBG.color = bgColor;
            }
            if (respawnTimer >= 0f)
            {
                respawnTimer -= Time.deltaTime;
                
            }
            else
            {
                ResetGame();
                SetGameState(StateType.respawn);
                respawnTimer = respawnTimerValue;
            }
        }
    }

    public void ResetGame()
    {
        // TODO: reset all variables to initials
        this.health = 90;
        this.score = 0;
        bossLives = 3;
        //triggeredGameEnd = false;
    }

    public void SetScore(int addToScore){
        score += addToScore;
    }

    public int GetScore()
    {
        return this.score;
    }

    public void TakeDamage()
    {
        health -= 90;
    }
    public void RemoveLife()
    {
        bossLives--;
    }

    public LevelManager GetLevelManager()
    {
        return LevelManager.instance;
    }

    public void SetCheckPoint(Vector2 point)
    {
        checkpointPos = point;
    }

    public Vector2 GetCheckPointPos()
    {
        return checkpointPos;
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
