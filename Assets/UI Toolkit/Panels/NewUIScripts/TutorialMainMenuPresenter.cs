using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMainMenuPresenter
{
    public Action OpenNewGame { set => _NewGameButton.clicked += value; }
    public Action OpenLoadGame { set => _LoadGameButton.clicked += value; }
    public Action OpenHighScore { set => _HighscoreButton.clicked += value; }
    public Action OpenCutscene { set => _CutscenesButton.clicked += value; }
    public Action OpenOptions { set => _OptionsButton.clicked += value; }
    public Action OpenCredits { set => _CreditsButton.clicked += value; }
    public Action OpenQuit { set => _QuitButton.clicked += value; }



    private Button _NewGameButton;
    private Button _LoadGameButton;
    private Button _HighscoreButton;
    private Button _CutscenesButton;
    private Button _OptionsButton;
    private Button _CreditsButton;
    private Button _QuitButton;

    public TutorialMainMenuPresenter(VisualElement root)
    {

        _NewGameButton = root.Q<Button>("NewGameButton");
        _LoadGameButton = root.Q<Button>("LoadGameButton");
        _HighscoreButton = root.Q<Button>("HighscoreButton");
        _CutscenesButton = root.Q<Button>("CutscenesButton");
        _OptionsButton = root.Q<Button>("OptionsButton");
        _CreditsButton = root.Q<Button>("CreditsButton");
        _QuitButton = root.Q<Button>("QuitButton");

        //AddLogsToButtons();
    }


    private void AddLogsToButtons()

    {
        _NewGameButton.clicked += () => { 
            Debug.Log("New Game Button clicked");
        };
        _LoadGameButton.clicked += () => Debug.Log("Load Game Button clicked");
        _HighscoreButton.clicked += () => Debug.Log("Highscore Button clicked");
        _CutscenesButton.clicked += () => Debug.Log("Cutscenes Button clicked");
        _OptionsButton.clicked += () => Debug.Log("Options Button clicked");
        _CreditsButton.clicked += () => Debug.Log("Credits Button clicked");
        _QuitButton.clicked += () => Debug.Log("Quit Button clicked");
    }


}
