using System;
using UnityEngine.UIElements;

public class BrightnessMenu
{
    private Button _backButton;
    public Action BackAction { set => _backButton.clicked += value; }

    public BrightnessMenu(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }
}
