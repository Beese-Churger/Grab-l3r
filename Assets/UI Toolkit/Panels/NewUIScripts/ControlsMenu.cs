using System;
using UnityEngine.UIElements;

public class ControlsMenu
{
    private Button _backButton;
    private Button GrappleBind;
    public Label gLabel;
    public Action BackAction { set => _backButton.clicked += value; }
    public Action GBind { set => GrappleBind.clicked += value; }

    public ControlsMenu(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        GrappleBind = root.Q<Button>("Grapple");
        gLabel = root.Q<Label>("GrappleLabel");
    }
}
