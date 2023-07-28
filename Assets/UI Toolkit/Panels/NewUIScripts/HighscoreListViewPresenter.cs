using UnityEngine.UIElements;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HighscoreListViewPresenter
{
    private Button _backButton;
    private int highscore;

    private void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", highscore);
    }

    public Action BackAction { set => _backButton.clicked += value; }

    public HighscoreListViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }
}
