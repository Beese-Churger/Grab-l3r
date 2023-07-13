using System;
using UnityEngine.UIElements;

public class ControlsMenu
{
    private Button _backButton;
    public Action BackAction { set => _backButton.clicked += value; }

    public ControlsMenu(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }
}
