using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialStartViewPresenter : MonoBehaviour
{
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



        SetupTutorialStartMenu();
        SetupTutorialLoadGameMenu();

        SetupCutscenesMenu();
        CutscenesToMM();

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
