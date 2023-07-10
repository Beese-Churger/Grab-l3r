using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialLoadGameViewPresenter
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

    public Action BackAction { set => _backButton.clicked += value; }

    private Button _backButton;
    private Toggle _fullscreenToggle;
    private DropdownField _resolutionSelection;

    public TutorialLoadGameViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _fullscreenToggle = root.Q<Toggle>("FullScreenToggle");
        _resolutionSelection = root.Q<DropdownField>("ResolutionDropDown");

        _fullscreenToggle.RegisterCallback<MouseUpEvent>((evt) => { SetFullscreen(_fullscreenToggle.value); }, TrickleDown.TrickleDown);
        _resolutionSelection.choices = _resolutions;
        _resolutionSelection.RegisterValueChangedCallback((value) => SetResolution(value.newValue));
        _resolutionSelection.index = 0;
    }

    private void SetResolution(string newResolution)
    {
        string[] resolutionArray = newResolution.Split("x");
        int[] valuesIntArray = new int[] { int.Parse(resolutionArray[0]), int.Parse(resolutionArray[1]) };

        Screen.SetResolution(valuesIntArray[0], valuesIntArray[1], _fullscreenToggle.value);
    }

    private void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }
}