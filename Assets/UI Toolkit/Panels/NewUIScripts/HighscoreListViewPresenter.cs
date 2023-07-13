using UnityEngine.UIElements;
using System;

public class HighscoreListViewPresenter
{
    private Button _backButton;
    public Action BackAction { set => _backButton.clicked += value; }

    public HighscoreListViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }
}
