using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.RebindUI;
using UnityEngine.InputSystem;

public class NewOptions : MonoBehaviour
{
    // Start is called before the first frame update
    public static NewOptions instance = null;

    private static bool isPressed = false;
    public static bool isPaused = false;
    private bool change = false;

    // Options GO prefab
    public InputActionAsset inputActions;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference switchToOptionsControls;
    [SerializeField] private InputActionReference ToggleOptionsScreen;

    [SerializeField] GameObject keyRebind;
    [SerializeField] public InputActionReference grappleRebind;
    [SerializeField] public InputActionReference movementRebind;
    [SerializeField] public InputActionReference jumpRebind;
    [SerializeField] public InputActionReference suicideRebind;

    private List<InputActionReference> allRebindRef;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            allRebindRef = new()
            {
                grappleRebind,
                jumpRebind,
                suicideRebind
            };
            playerInput.SwitchCurrentActionMap("Options");
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
        if (LevelManager.instance.GetCurrentLevelIndex() > 1)
        {
            if ((switchToOptionsControls.action.triggered || ToggleOptionsScreen.action.triggered) && !isPressed)
            {
                if (isPaused)
                {
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    Time.timeScale = 1;
                    isPaused = false;
                }

                else
                {
                    // Switch to Options action map to prevent the player from controlling it
                    playerInput.SwitchCurrentActionMap("Options");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 0;
                    isPaused = true;
                }

                isPressed = true;
            }
            else
                isPressed = false;

            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    GameManager.instance.SetGameState(StateType.levelChange);
            //    EnemyManager.enemyManager.ClearEnemyList();
            //}
            if (suicideRebind.action.triggered && GameManager.instance.GetCurrentPlayerHealth() > 0)
            {
                GameManager.instance.InstantDeath();
            }
        }
        CheckCurrentInput();

    }
    public bool GetPauseState()
    {
        return isPaused;
    }
    public void SetPauseState(bool state)
    {
        isPaused = state;
        isPressed = state;
        Debug.Log(isPaused);
    }
    public void SetPlayerInput(string actionMapName)
    {
        playerInput.SwitchCurrentActionMap(actionMapName);
    }
    public void BindKey(InputActionReference inputActionReference)
    {
        SetPlayerInput("Options");

        Keybind temp = keyRebind.GetComponent<Keybind>();
        temp.actionReference = inputActionReference;
        temp.bindingId = inputActionReference.action.bindings[0].id.ToString();
        temp.StartInteractiveRebind();
        //Debug.Log("rebinding");

    }
    public void BindingDone(string newActionName, InputActionReference actionRef)
    {
        if (grappleRebind == actionRef)
            ControlsMenu.instance.gLabel.text = newActionName;
        else if (jumpRebind == actionRef)
            if (newActionName != "Space")
                ControlsMenu.instance.jLabel.text = "Space";
            else
                ControlsMenu.instance.jLabel.text = newActionName;
        else if (suicideRebind == actionRef)
            ControlsMenu.instance.sLabel.text = newActionName;

        //Debug.Log("done rebinding");
        RebindLoadSave.SaveKeybind();
    }
    public void RefreshDisplay()
    {
        Keybind temp = keyRebind.GetComponent<Keybind>();
        foreach (InputActionReference actionRef in allRebindRef)
        {
            temp.actionReference = actionRef;
            temp.bindingId = actionRef.action.bindings[0].id.ToString();
            temp.UpdateBindingDisplay();
        }
    }
    public void CheckCurrentInput()
    {
        if (playerInput.currentActionMap.name == "Gameplay")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
