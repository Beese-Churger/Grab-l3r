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
    public static GameManager instance = null;

    private float health = 90;
    private int score = 0;
    private bool gamePaused;
    private bool triggeredGameEnd;
    private StateType state;
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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
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
            case StateType.boss:
                StartCoroutine(levelManager.LoadLevel("LevelLayout Boss"));
                break;
        }
    }

    public StateType GetGameState()
    {
        return state;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
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
