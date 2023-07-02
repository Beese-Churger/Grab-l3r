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
    public static GameManager instance = null;
    private float health = 90;
    private int score = 0;
    private bool triggeredGameEnd;
    public StateType state;

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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
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
        this.health = 90;
        this.score = 0;
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

    public LevelManager GetLevelManager()
    {
        return LevelManager.instance;
    }

}
