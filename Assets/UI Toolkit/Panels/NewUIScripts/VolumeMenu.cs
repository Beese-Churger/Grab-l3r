using System;
using UnityEngine.UIElements;

public class VolumeMenu
{
    private Button _backButton;
    public Action BackAction { set => _backButton.clicked += value; }

    public VolumeMenu (VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }

}
