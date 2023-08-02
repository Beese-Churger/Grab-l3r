using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;



public class Options : MonoBehaviour
{
    public static Options instance = null;

    private float w, h;
    private static bool isPressed = false;
    public static bool isPaused = false;
    // Options GO prefab
    [SerializeField] private GameObject OptionsMenu;

    // Panel GO
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject SoundPanel;
    [SerializeField] private GameObject VideoPanel;
    [SerializeField] private GameObject ButtonPanel;
    [SerializeField] private GameObject ControlPanel;
    private List<GameObject> panelList;
  

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference switchToOptionsControls;
    [SerializeField] private InputActionReference ToggleOptionsScreen;

    // Resolution Variables
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    
    // Fullscreen / Windowed Mode toggle variables

    [SerializeField] private TMP_Dropdown fullscreenDropdown;


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
        panelList = new List<GameObject>
        {
            SoundPanel,
            VideoPanel,
            ControlPanel
        };
        // Toggling between fullscreen and windowed
        // Set the default option based on the current screen mode
        fullscreenDropdown.value = Screen.fullScreen ? 0 : 1;
        fullscreenDropdown.RefreshShownValue();
        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChanged);
        //----------------------------------------------------------------------------------------------------------------
        // RESOLUTION
        //----------------------------------------------------------------------------------------------------------------
        // Get the available screen resolutions
        resolutions = Screen.resolutions;

        // Clear existing options from the dropdown
        resolutionDropdown.ClearOptions();

        // Create a new list to store resolution labels
        var options = new List<string>();

        // Populate the options list with resolution labels
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
        }

        // Add the options to the dropdown
        resolutionDropdown.AddOptions(options);

        // Set the default resolution option based on the current screen resolution
        int currentResolutionIndex = GetCurrentResolutionIndex();
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Assign a listener to the dropdown's value changed event
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        //----------------------------------------------------------------------------------------------------------------
        //Audio Sliders for BGM and SFX
        //if (AudioManager.Instance != null)
        //{
        //    AudioManager.Instance.SFXvolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SFXVolume);
        //    AudioManager.Instance.BGMvolumeSlider.onValueChanged.AddListener(AudioManager.Instance.BGMVolume);

        //}

    }

    public void OnResolutionChanged(int resolutionIndex)
    {
        // Set the selected resolution
        Resolution selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        StartCoroutine(Wait());
        SetCanvasSize();
        Debug.Log("Changed resolution to " + selectedResolution);
    }

    private int GetCurrentResolutionIndex()
    {
        // Find the index of the current screen resolution in the available resolutions array
        Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }

        // If the current resolution is not found, return 0 as the default index
        return 0;
    }
    public void OnFullscreenChanged(int optionIndex)
    {
        // Toggle between fullscreen and windowed mode based on the selected option
        bool isFullscreen = optionIndex == 0;
        Screen.fullScreen = isFullscreen;
    }
    public void SetCanvasSize()
    {
        w = rectTransform.rect.width * 0.5f;
        h = rectTransform.rect.height * 0.5f;
        rectTransform.rect.Set(w, h, w, h);
    }

    private void Update()
    {
        if (LevelManager.instance.GetCurrentLevelIndex() > 1)
        {
            if ((switchToOptionsControls.action.triggered || ToggleOptionsScreen.action.triggered) && !isPressed)
            {
                if (OptionsMenu.activeSelf)
                {
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    CheckActivePanel();
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

                OptionsMenu.SetActive(!OptionsMenu.activeSelf);
                ButtonPanel.SetActive(!ButtonPanel.activeSelf);
                isPressed = true;
            }
            else
                isPressed = false;

            if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.instance.SetGameState(StateType.levelChange);
                EnemyManager.enemyManager.ClearEnemyList();
            }
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
    }
    public void ToggleVideoPanel()
    {
        ButtonPanel.SetActive(!ButtonPanel.activeSelf);
        VideoPanel.SetActive(!VideoPanel.activeSelf);
    }
    public void ToggleSoundPanel()
    {
        ButtonPanel.SetActive(!ButtonPanel.activeSelf);
        SoundPanel.SetActive(!SoundPanel.activeSelf);
    }
    public void ToggleControlPanel()
    {
        ButtonPanel.SetActive(!ButtonPanel.activeSelf);
        ControlPanel.SetActive(!ControlPanel.activeSelf);
    }
    public void TriggerReset()
    {
        
        CheckActivePanel();
        OptionsMenu.SetActive(false);
        ButtonPanel.SetActive(!ButtonPanel.activeSelf);
        playerInput.SwitchCurrentActionMap("Gameplay");
        Time.timeScale = 1;      
        GameManager.instance.SetGameState(StateType.respawn);
    }
    public void TriggerQuit()
    {
        CheckActivePanel();
        OptionsMenu.SetActive(false);
        ButtonPanel.SetActive(!ButtonPanel.activeSelf);
        playerInput.SwitchCurrentActionMap("Gameplay");
        Time.timeScale = 1;

        AudioManager.Instance.PlayBGMLoop("level1bgm", true);
        LevelManager.instance.SetCurrentLevelIndex(0);
        GameManager.instance.SetGameState(StateType.end);
    }
    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
    public void CheckActivePanel()
    {       
        foreach (GameObject panel in panelList)
        {
            if (panel.activeSelf)
                panel.SetActive(false);
        }
        ButtonPanel.SetActive(true);       
    }
    public bool GetPauseState()
    {
        return isPaused;
    }
}
