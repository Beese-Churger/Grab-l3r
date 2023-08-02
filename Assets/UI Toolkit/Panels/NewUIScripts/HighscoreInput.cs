using System;
using UnityEngine;
using UnityEngine.UIElements;
public class HighscoreInput : MonoBehaviour
{
    public TextField textField;
    private Button cancelButton;
    private Button okButton;

    public Action OkButtonDown { set => okButton.clicked += value; }
    public Action CancelButtonDown { set => cancelButton.clicked += value; }
    public HighscoreInput(VisualElement root)
    {
        textField = root.Q<TextField>("HighscoreField");
        cancelButton = root.Q<Button>("CancelButton");
        okButton = root.Q<Button>("OKButton");

        textField.value = RandomNameGenerator.GenInstance.GetRandomName();
    }
}
