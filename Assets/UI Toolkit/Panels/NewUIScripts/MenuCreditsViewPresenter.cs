using UnityEngine.UIElements;
using System;

public class MenuCreditsViewPresenter 
{
    private VisualElement _credits;
    private VisualElement _startView;

    private Button _backButton;

    public Action BackAction { set => _backButton.clicked += value; }

    // Start is called before the first frame update
    public MenuCreditsViewPresenter(VisualElement root)
    {
        _startView = root.Q("TutorialMainMenu");
        _credits = root.Q("MenuCreditsView");
        _backButton = root.Q<Button>("BackButton");

    }
}
