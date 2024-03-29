using System;
using UnityEngine.UIElements;

public class TutorialLoadGameViewPresenter
{
    public Action LoadLevel1 { set => _level1Button.clicked += value; }
    public Action LoadLevel2 { set => _level2Button.clicked += value; }
    public Action LoadLevel3 { set => _level3Button.clicked += value; }

    public Action BackAction { set => _backButton.clicked += value; }

    private Button _backButton;
    private Button _level1Button;
    private Button _level2Button;
    private Button _level3Button;

    public VisualElement lock1;
    public VisualElement lock2;
    public VisualElement lock3;



    public TutorialLoadGameViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _level1Button = root.Q<Button>("Level1Button");
        _level2Button = root.Q<Button>("Level2Button");
        _level3Button = root.Q<Button>("Level3Button");

        lock1 = root.Q("LockElement1");
        lock2 = root.Q("LockElement2");
        lock3 = root.Q("LockElement3");


    }
}
