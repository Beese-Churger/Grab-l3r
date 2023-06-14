using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Temporary until GameManager Works
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;



public class Options : MonoBehaviour
{


    private float w, h;
    private static bool isPressed = false;
    // Options GO prefab
    [SerializeField] private GameObject OptionsMenu;

    // Panel GO
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject SoundPanel;
    [SerializeField] private GameObject VideoPanel;
    [SerializeField] private GameObject ButtonPanel;
    [SerializeField] private GameObject ControlPanel;
  

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference switchToOptionsControls;
    [SerializeField] private InputActionReference ToggleOptionsScreen;
    private InputActionMap controlActionMap;

    // Resolution Variables
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    
    // Fullscreen / Windowed Mode toggle variables

    [SerializeField] private TMP_Dropdown fullscreenDropdown;


    // For the reset button
    [SerializeField] private string sceneName;


    // Key binding
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputAction currentAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private void Start()
    {
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
        AudioManager.Instance.SFXvolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SFXVolume);
        AudioManager.Instance.BGMvolumeSlider.onValueChanged.AddListener(AudioManager.Instance.BGMVolume);

        //----------------------------------------------------------------------------------------------------------------
        controlActionMap = playerInput.currentActionMap;
        // Check for which action I'm currently changing the keybind for
        currentAction = controlActionMap.actions[0];
        //int bindingIndex = currentAction.GetBindingIndexForControl(currentAction.controls[0]);
        //bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
        //        currentAction.bindings[bindingIndex].effectivePath,
        //        InputControlPath.HumanReadableStringOptions.OmitDevice);


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
        if ((switchToOptionsControls.action.triggered || ToggleOptionsScreen.action.triggered) && !isPressed)
        {
            if (OptionsMenu.activeSelf)
            {
                playerInput.SwitchCurrentActionMap("Gameplay");
                Time.timeScale = 1;
            }

            else
            {
                // Switch to Options action map to prevent the player from controlling it
                playerInput.SwitchCurrentActionMap("Options");
                Time.timeScale = 0;
            }

            OptionsMenu.SetActive(!OptionsMenu.activeSelf);
            ButtonPanel.SetActive(!ButtonPanel.activeSelf);
            isPressed = true;
        }
        else
            isPressed = false;
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
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);        
    }
    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        rebindingOperation = currentAction.PerformInteractiveRebinding()
                .WithExpectedControlType("Button")
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
    }
    public void RebindComplete()
    {
        int bindingIndex = currentAction.GetBindingIndexForControl(currentAction.controls[0]);
        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
                currentAction.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);


        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);

    }
    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

}
