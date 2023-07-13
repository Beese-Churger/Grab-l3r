using System;
using UnityEngine.UIElements;

public class OptionsMenuViewPresenter
{
    private VisualElement _options;
    private VisualElement _volume;


    private Button _volumeButton;
    private Button _brightnessButton;
    private Button _ResolutionButton;
    private Button _controlsButton;
    private Button _difficultyButton;
    private Button _backButton;

    public Action BackAction { set => _backButton.clicked += value; }
    public Action OpenVolume { set => _volumeButton.clicked += value; }
    public Action OpenBrightness { set => _brightnessButton.clicked += value; }
    public Action OpenResolution { set => _ResolutionButton.clicked += value; }
    public Action OpenControls { set => _controlsButton.clicked += value; }
    //public Action OpenDifficulty { set => _difficultyButton.clicked += value; }


    public OptionsMenuViewPresenter (VisualElement root)
    {
        _options = root;
        _volumeButton = root.Q<Button>("VolumeButton");
        _brightnessButton = root.Q<Button>("BrightnessButton");
        _ResolutionButton = root.Q<Button>("ResolutionButton");
        _controlsButton = root.Q<Button>("ShowControlsButton");
        _difficultyButton = root.Q<Button>("");
        _backButton = root.Q<Button>("BackButton");

        _volume = root.Q("VolumeMenu");

    }

}
