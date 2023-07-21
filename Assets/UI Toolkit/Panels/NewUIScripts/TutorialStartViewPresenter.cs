using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEngine.InputSystem.RebindUI;
using UnityEngine.InputSystem;


public class TutorialStartViewPresenter : MonoBehaviour
{
    public static TutorialStartViewPresenter instance = null;

    public VideoPlayer vp;
    private VisualElement _loadgameView;
    private VisualElement _startView;
    private VisualElement _highScore;
    private VisualElement _cutScenes;
    private VisualElement _options;
    private VisualElement _credits;
    private VisualElement _quitconfirm;

    private VisualElement _volumeScreen;
    private VisualElement _brightnessScreen;
    private VisualElement _resolutionScreen;
    private VisualElement _controlScreen;

    private VisualElement _rebindOverlay;

    [SerializeField] GameObject keyRebind;
    [SerializeField] public InputActionReference grappleRebind;
    [SerializeField] public InputActionReference movementRebind;
    [SerializeField] public InputActionReference jumpRebind;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        // name = the name of the xml file ref
        _startView = root.Q("TutorialMainMenu");
        _loadgameView = root.Q("TutorialLoadGameView");
        _highScore = root.Q("HighscoreList");
        _cutScenes = root.Q("CutscenesMenu");
        _options = root.Q("OptionsMenu");
        _credits = root.Q("MenuCredits");
        _quitconfirm = root.Q("QuitConfirmationMenu");

        _volumeScreen = root.Q("VolumeMenu");
        _brightnessScreen = root.Q("BrightnessMenu");
        _resolutionScreen = root.Q("ResolutionDropdownMenu");
        _controlScreen = root.Q("ControlsMenu");

        _rebindOverlay = root.Q("RebindOverlay");


        vp.loopPointReached += CutsceneOver;

        SetupTutorialStartMenu();
        SetupTutorialLoadGameMenu();
        LoadLevel1();
        LoadLevel2();
        LoadLevel3();

        SetupCutscenesMenu();
        CutscenesToMM();
        PlayIntroCutscene();
        PlayEndingCutscene();

        SetupQuitConfirm();
        ExitQuitConfirm();
        CancelQuitConfirm();
        QuitConfirmed();

        OpenCredits();
        ExitCredits();

        OpenHighscoreMenu();
        CloseHighscoreMenu();

        OpenOptionsMenu();
        CloseOptionsMenu();

        OpenVolumeMenu();
        CloseVolumeMenu();

        OpenBrightnessMenu();
        CloseBrightnessMenu();

        OpenResolutionMenu();
        CloseResolutionMenu();

        OpenControlsMenu();
        CloseControlsMenu();
        
        //BindKey();

        OpenNewGame();

        GameManager.instance.GetLevelManager().PlayLevelBGM(false);
    }

    // OPEN NEW GAME
    private void OpenNewGame()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenNewGame = () =>
        {
            _startView.Display(false);
            GameManager.instance.SetGameState(StateType.levelChange);
        };
    }

    // OPENS LOAD GAME MENU
    private void SetupTutorialStartMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenLoadGame = () => {
            ToggleLoadGameMenu(true);
            Debug.Log("Inside Load Game Menu");
        };
    }
    // CLOSE LOAD GAME MENU
    private void SetupTutorialLoadGameMenu()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.BackAction = () => ToggleLoadGameMenu(false);
    }
    private void SetupCutscenesMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenCutscene = () =>
        {
            ToggleCutscenesMenu(true);
            Debug.Log("Inside Cutscenes");
        };
    }
    private void CutscenesToMM()
    {
        CutscenesMenu cutscenesViewPresenter = new(_cutScenes);
        cutscenesViewPresenter.BackAction = () => ToggleCutscenesMenu(false);
    }
    // Open Quit confirmation menu
    private void SetupQuitConfirm()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenQuit = () => ToggleQuitMenu(true);
    }
    // Close Quit confirmation menu via Back Button
    private void ExitQuitConfirm()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.BackAction = () => ToggleQuitMenu(false);
    }
    // Close Quit confirmation menu via cancel button
    private void CancelQuitConfirm()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.CancelAction = () => ToggleQuitMenu(false);
    }
    // Quit Application
    private void QuitConfirmed()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.QuitAction = () => Quit();
    }
    // Open Credits Menu
    private void OpenCredits()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenCredits = () => ToggleCreditsMenu(true);
    }
    // Close Credits Menu
    private void ExitCredits()
    {
        MenuCreditsViewPresenter menuCreditsViewPresenter = new(_credits);
        menuCreditsViewPresenter.BackAction = () => ToggleCreditsMenu(false);
    }
    // Open Highscore Menu
    private void OpenHighscoreMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenHighScore = () => ToggleHighscoreList(true);
    }
    // Close Highscore Menu
    private void CloseHighscoreMenu()
    {
        HighscoreListViewPresenter highscoreListViewPresenter = new(_highScore);
        highscoreListViewPresenter.BackAction = () => ToggleHighscoreList(false);
    }
    // Open options menu
    private void OpenOptionsMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenOptions = () => ToggleOptionsMenu(true);
    }
    private void CloseOptionsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.BackAction = () => ToggleOptionsMenu(false);
    }
    private void OpenVolumeMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenVolume = () => ToggleVolumeMenu(true);
    }
    private void CloseVolumeMenu()
    {
        VolumeMenu volumeMenu = new(_volumeScreen);
        volumeMenu.BackAction = () => ToggleVolumeMenu(false);
    }
    private void OpenBrightnessMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenBrightness = () => ToggleBrightnessMenu(true);
    }
    private void CloseBrightnessMenu()
    {
        BrightnessMenu brightnessMenu = new(_brightnessScreen);
        brightnessMenu.BackAction = () => ToggleBrightnessMenu(false);
    }
    private void OpenResolutionMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenResolution = () => ToggleResolutionScreen(true);
    }
    private void CloseResolutionMenu()
    {
        ResolutionMenu resolutionMenu = new(_resolutionScreen);
        resolutionMenu.BackAction = () => ToggleResolutionScreen(false);
    }
    private void OpenControlsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenControls = () => ToggleControlScreen(true);
    }
    private void CloseControlsMenu()
    {
        ControlsMenu controlsMenu = new(_controlScreen);
        controlsMenu.BackAction = () => ToggleControlScreen(false);
    }

    public void BindKey(InputActionReference inputActionReference)
    {
        Keybind temp = keyRebind.GetComponent<Keybind>();
        temp.actionReference = inputActionReference;
        temp.bindingId = inputActionReference.action.bindings[0].id.ToString();
        temp.StartInteractiveRebind();
        _controlScreen.Display(false);
        _rebindOverlay.Display(true);

    }
    public void BindingDone(string newActionName)
    {
        ControlsMenu controlsMenu = new(_controlScreen);
        if (keyRebind.GetComponent<Keybind>().actionReference == grappleRebind)
            controlsMenu.gLabel.text = keyRebind.GetComponent<Keybind>().actionReference.action.name + "                 " + newActionName;
        else if (keyRebind.GetComponent<Keybind>().actionReference == movementRebind)
            controlsMenu.mLabel.text = "Up/Left/Down/Right" + "        " + newActionName;
        else if (keyRebind.GetComponent<Keybind>().actionReference == jumpRebind)
            controlsMenu.jLabel.text = keyRebind.GetComponent<Keybind>().actionReference.action.name + "                 " + newActionName;



        _controlScreen.Display(true);
        _rebindOverlay.Display(false);
    }

    // LOAD LEVELS
    private void LoadLevel1()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel1 = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout")].Completed())
                StartCoroutine(LevelManager.instance.LoadLevel("LevelLayout"));
            else
                Debug.Log("Level 1 not unlocked ");
        };
    }
    private void LoadLevel2()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel2 = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout 2")].Completed())
                StartCoroutine(LevelManager.instance.LoadLevel("LevelLayout 2")); 
            else
                Debug.Log("Level 2 not unlocked ");

        };
    }
    private void LoadLevel3()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel3 = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout Boss")].Completed())
                StartCoroutine(LevelManager.instance.LoadLevel("LevelLayout Boss"));
            else
                Debug.Log("Boss Level not unlocked ");
        };
    }
    // Play Cutscenes
    private void PlayIntroCutscene()
    {
        CutscenesMenu cutscenesMenu = new(_cutScenes);
        cutscenesMenu.PlayIntro = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("Level1Cutscene")].Completed())
            // Play the cutscene when its unlocked
            {
                Debug.Log("Play Intro Cutscene");
                vp.Play();
                _cutScenes.Display(false);
                GameManager.instance.GetLevelManager().PlayLevelBGM(true);

            }
            else
            {
                Debug.Log("Intro Cutscene not unlocked");
            }
        };
    }
    private void PlayEndingCutscene()
    {
        CutscenesMenu cutscenesMenu = new(_cutScenes);
        cutscenesMenu.PlayEnding = () => { 
            // Play the cutscene when its unlocked
        };

    }
    private void CutsceneOver(VideoPlayer vp)
    {
        vp.Stop();
        _cutScenes.Display(true);
        GameManager.instance.GetLevelManager().PlayLevelBGM(false);

    }
    /// <summary>
    /// Toggle between Main Menu Screen and other available screens
    /// </summary>
    /// <param name="enable"></param>

    private void ToggleLoadGameMenu(bool enable)
    {
        _startView.Display(!enable);
        _loadgameView.Display(enable);
    }
    private void ToggleHighscoreList(bool enable)
    {
        _startView.Display(!enable);
        _highScore.Display(enable);
    }
    private void ToggleCutscenesMenu(bool enable)
    {
        _startView.Display(!enable);
        _cutScenes.Display(enable);
    }
    private void ToggleCreditsMenu(bool enable)
    {
        _startView.Display(!enable);
        _credits.Display(enable);
    }
    private void ToggleQuitMenu(bool enable)
    {
        _startView.Display(!enable);
        _quitconfirm.Display(enable);

    }
    private void ToggleOptionsMenu(bool enable)
    {
        _startView.Display(!enable);
        _options.Display(enable);
    }

    private void ToggleVolumeMenu(bool enable)
    {
        _options.Display(!enable);
        _volumeScreen.Display(enable);
    }
    private void ToggleBrightnessMenu(bool enable)
    {
        _options.Display(!enable);
        _brightnessScreen.Display(enable);
    }
    private void ToggleResolutionScreen(bool enable)
    {
        _options.Display(!enable);
        _resolutionScreen.Display(enable);
    }
    private void ToggleControlScreen(bool enable)
    {
        _options.Display(!enable);
        _controlScreen.Display(enable);
    }






    private void Quit()
    {
        Application.Quit();
    }
}
