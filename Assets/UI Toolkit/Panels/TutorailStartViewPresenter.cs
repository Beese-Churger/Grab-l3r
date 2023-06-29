using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialStartViewPresenter : MonoBehaviour
{
    private VisualElement _loadgameView;
    private VisualElement _startView;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _startView = root.Q("TutorialMainMenu");
        _loadgameView = root.Q("TutorialLoadGameView");

        SetupTutorialStartMenu();
        SetupLoadGameMenu();
    }

    private void SetupTutorialStartMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new TutorialMainMenuPresenter();
        menuPresenter.OpenLoadGameButton = () => ToggleSettingsMenu(true);
    }

    private void SetupLoadGameMenu()
    {
        TutorialLoadGamePresenter loadgamePresenter = new TutorialLoadGamePresenter();
        loadgamePresenter.BackAction = () => ToggleSettingsMenu(false);
    }

    private void ToggleSettingsMenu(bool enable)
    {
        _startView.Display(!enable);
        _loadgameView.Display(enable);
     }
}
