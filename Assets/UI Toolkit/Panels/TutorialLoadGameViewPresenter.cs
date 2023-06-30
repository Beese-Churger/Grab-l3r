using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialLoadGameViewPresenter
{
    public Action BackAction { set => _backButton.clicked += value; }

    private Button _backButton;

    public TutorialLoadGameViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
    }
}
