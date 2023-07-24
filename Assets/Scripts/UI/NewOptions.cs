using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NewOptions : MonoBehaviour
{
    // Start is called before the first frame update
    public static NewOptions instance = null;

    private static bool isPressed = false;
    public static bool isPaused = false;
    private bool change = false;

    // Options GO prefab
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference switchToOptionsControls;
    [SerializeField] private InputActionReference ToggleOptionsScreen;

    [SerializeField] private InputActionReference suicide;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        isPaused = false;
    }


    private void Start()
    {
       

    }
    private void Update()
    {
        if (LevelManager.instance.GetCurrentLevelIndex() > 0)
        {
            if ((switchToOptionsControls.action.triggered || ToggleOptionsScreen.action.triggered) && !isPressed || change)
            {
                if (isPaused)
                {
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    Time.timeScale = 1;
                    isPaused = false;
                }

                else
                {
                    // Switch to Options action map to prevent the player from controlling it
                    playerInput.SwitchCurrentActionMap("Options");
                    Time.timeScale = 0;
                    isPaused = true;
                }

                isPressed = true;
                change = false;
            }
            else
                isPressed = false;

            if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.instance.SetGameState(StateType.levelChange);
                EnemyManager.enemyManager.ClearEnemyList();
            }
            if (suicide.action.triggered && GameManager.instance.GetCurrentPlayerHealth() > 0)
            {
                GameManager.instance.InstantDeath();
            }
        }
    }
    public bool GetPauseState()
    {
        return isPaused;
    }
    public void SetPauseState(bool state)
    {
        change = !state;
    }
    public void SetPlayerInput(string actionMapName)
    {
        playerInput.SwitchCurrentActionMap(actionMapName);
    }


}
