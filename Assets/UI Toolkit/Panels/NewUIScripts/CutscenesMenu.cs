using System;
using UnityEngine.UIElements;

public class CutscenesMenu
{
    private Button _backButton;
    public Button _playEndingButton;
    public Button _playIntroButton;

    public Action BackAction { set => _backButton.clicked += value; }
    public Action PlayIntro { set => _playIntroButton.clicked += value; }
    public Action PlayEnding { set => _playEndingButton.clicked += value; }

    public CutscenesMenu(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _playIntroButton = root.Q<Button>("PlayIntroButton");
        _playEndingButton = root.Q<Button>("PlayEndingButton");
    }
}
