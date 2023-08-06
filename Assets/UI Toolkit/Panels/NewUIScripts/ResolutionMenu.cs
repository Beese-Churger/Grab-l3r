using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionMenu
{
    private List<string> _resolutions = new List<string>()
    {
        "1360x768",
        "1366x768",
        "1440x900",
        "1600x900",
        "1600x1024",
        "1680x1050",
        "1920x1080"
    };

    private Button _backButton;
    private Toggle _fullscreenToggle;
    private DropdownField _resolutionSelection;

    public Action BackAction { set => _backButton.clicked += value; }

    public ResolutionMenu(VisualElement root)
    {    
        _backButton = root.Q<Button>("BackButton");
        _fullscreenToggle = root.Q<Toggle>("FullScreenToggle");
        _resolutionSelection = root.Q<DropdownField>("ResolutionDropDown");
        _resolutionSelection.choices = _resolutions;

        _fullscreenToggle.value = NewOptions.instance.isFullscreen;
        _resolutionSelection.index = NewOptions.instance.GetCurrentResolutionIndex();
        SetResolution(NewOptions.instance.res);
        _fullscreenToggle.RegisterCallback<MouseUpEvent>((evt) => { SetFullscreen(_fullscreenToggle.value); });
        _resolutionSelection.RegisterValueChangedCallback((value) => {
            SetResolution(value.newValue);
        });
          
    }
    private void SetResolution(string newResolution)
    {
        NewOptions.instance.res = newResolution;
        string[] resolutionArray = newResolution.Split("x");
        int[] valuesIntArray = new int[] { int.Parse(resolutionArray[0]), int.Parse(resolutionArray[1]) };
        Screen.SetResolution(valuesIntArray[0], valuesIntArray[1], _fullscreenToggle.value);
    }

    private void SetFullscreen(bool val)
    {
        Screen.fullScreen = val;
        NewOptions.instance.isFullscreen = val;
    }
}
