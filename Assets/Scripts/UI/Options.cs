using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Options : MonoBehaviour
{
    private float w, h;
    // Options GO prefab
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private RectTransform rectTransform;


    // Resolution Variables
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private bool isOpened = true;
    //
    // Fullscreen / Windowed Mode toggle variables

    [SerializeField] private TMP_Dropdown fullscreenDropdown;



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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpened = !isOpened;
            OptionsMenu.SetActive(isOpened);
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
    }
}
