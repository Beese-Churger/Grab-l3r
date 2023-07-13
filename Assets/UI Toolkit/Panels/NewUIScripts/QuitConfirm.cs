using UnityEngine.UIElements;
using UnityEngine;
using System;

public class QuitConfirm : MonoBehaviour
{
    private Button _yesButton; 
    private Button _noButton;
    private Button _backButton;

    public Action BackAction { set => _backButton.clicked += value; }
    public Action CancelAction { set => _noButton.clicked += value; }
    public Action QuitAction { set => _yesButton.clicked += value; }

    public QuitConfirm(VisualElement root)
    {
        _yesButton = root.Q<Button>("SureQuitButton");
        _noButton = root.Q<Button>("CancelButton");
        _backButton = root.Q<Button>("BackButton");
    }



}
