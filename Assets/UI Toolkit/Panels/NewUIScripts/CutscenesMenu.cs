using System;
using UnityEngine.UIElements;

public class CutscenesMenu
{
    private VisualElement _cutscenesView;
    private VisualElement _startView;


    private Button _backButton;
    private Button _playEndingButton;
    private Button _playIntroButton;

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
