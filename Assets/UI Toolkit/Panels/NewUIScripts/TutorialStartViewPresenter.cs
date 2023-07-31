using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;



public class TutorialStartViewPresenter : MonoBehaviour
{
    public static TutorialStartViewPresenter instance = null;

    public VideoPlayer vp;
    [SerializeField] private VideoClip startCutscene;
    [SerializeField] private VideoClip endCutscene;

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
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            _startView.Display(false);
            GameManager.instance.SetGameState(StateType.levelChange);
        };
    }

    // OPENS LOAD GAME MENU
    private void SetupTutorialStartMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenLoadGame = () => {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleLoadGameMenu(true);
            CheckLock();
            Debug.Log("Inside Load Game Menu");
        };
    }
    // CLOSE LOAD GAME MENU
    private void SetupTutorialLoadGameMenu()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleLoadGameMenu(false);
        };
    }
    private void SetupCutscenesMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenCutscene = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleCutscenesMenu(true);
            Debug.Log("Inside Cutscenes");
        };
    }
    private void CutscenesToMM()
    {
        CutscenesMenu cutscenesViewPresenter = new(_cutScenes);
        cutscenesViewPresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleCutscenesMenu(false);
        };
    }
    // Open Quit confirmation menu
    private void SetupQuitConfirm()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenQuit = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleQuitMenu(true);
        };
    }
    // Close Quit confirmation menu via Back Button
    private void ExitQuitConfirm()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleQuitMenu(false);
        };
    }
    // Close Quit confirmation menu via cancel button
    private void CancelQuitConfirm()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.CancelAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);
            ToggleQuitMenu(false);
        };
    }
    // Quit Application
    private void QuitConfirmed()
    {
        QuitConfirm quitConfirm = new(_quitconfirm);
        quitConfirm.QuitAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            Quit();
        };
    }
    // Open Credits Menu
    private void OpenCredits()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenCredits = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleCreditsMenu(true);
        };
    }
    // Close Credits Menu
    private void ExitCredits()
    {
        MenuCreditsViewPresenter menuCreditsViewPresenter = new(_credits);
        menuCreditsViewPresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleCreditsMenu(false);
        };
    }
    // Open Highscore Menu
    private void OpenHighscoreMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenHighScore = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleHighscoreList(true);
        };
    }
    // Close Highscore Menu
    private void CloseHighscoreMenu()
    {
        HighscoreListViewPresenter highscoreListViewPresenter = new(_highScore);
        highscoreListViewPresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleHighscoreList(false);
        };
    }
    // Open options menu
    private void OpenOptionsMenu()
    {
        TutorialMainMenuPresenter menuPresenter = new(_startView);
        menuPresenter.OpenOptions = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleOptionsMenu(true);
        };
    }
    private void CloseOptionsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleOptionsMenu(false);
        };
    }
    private void OpenVolumeMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenVolume = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleVolumeMenu(true);
        };
    }
    private void CloseVolumeMenu()
    {
        VolumeMenu volumeMenu = new(_volumeScreen);
        volumeMenu.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleVolumeMenu(false);
        };
    }
    private void OpenBrightnessMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenBrightness = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleBrightnessMenu(true);
        };
    }
    private void CloseBrightnessMenu()
    {
        BrightnessMenu brightnessMenu = new(_brightnessScreen);
        brightnessMenu.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleBrightnessMenu(false);
        };
    }
    private void OpenResolutionMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenResolution = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleResolutionScreen(true);
        };
    }
    private void CloseResolutionMenu()
    {
        ResolutionMenu resolutionMenu = new(_resolutionScreen);
        resolutionMenu.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleResolutionScreen(false);
        };
    }
    private void OpenControlsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_options);
        optionsMenuViewPresenter.OpenControls = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleControlScreen(true);
        };
    }
    private void CloseControlsMenu()
    {
        ControlsMenu controlsMenu = new(_controlScreen);
        controlsMenu.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            ToggleControlScreen(false);
        };
    }

    // LOAD LEVELS
    private void LoadLevel1()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel1 = () => {
            //Debug.Log(LevelManager.instance.GetLevelIndexWithName("Level_forestTutorial"));
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("Level_forestTutorial")].Completed())
            {
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                StartCoroutine(LevelManager.instance.LoadLevel("Level_forestTutorial"));
                GameManager.instance.GetLevelManager().PlayLevelBGM(false);
            }
            else
                AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);
        };
    }
    private void LoadLevel2()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel2 = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout 2")].Completed())
            {
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                StartCoroutine(LevelManager.instance.LoadLevel("LevelLayout 2"));
                GameManager.instance.GetLevelManager().PlayLevelBGM(false);
            }
            else
                AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);

        };
    }
    private void LoadLevel3()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        loadgamePresenter.LoadLevel3 = () => {
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout Boss")].Completed())
            {
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                StartCoroutine(LevelManager.instance.LoadLevel("LevelLayout Boss"));
                GameManager.instance.GetLevelManager().PlayLevelBGM(false);
            }
            else
                AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);
        };
    }
    private void CheckLock()
    {
        TutorialLoadGameViewPresenter loadgamePresenter = new(_loadgameView);
        if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("Level_forestTutorial")].Completed())
            loadgamePresenter.lock1.style.opacity = 0f;
        if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout 2")].Completed())
            loadgamePresenter.lock2.style.opacity = 0f;
        if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("LevelLayout Boss")].Completed())
            loadgamePresenter.lock3.style.opacity = 0f;
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
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                vp.clip = startCutscene;
                vp.Play();
                _cutScenes.Display(false);
                GameManager.instance.GetLevelManager().PlayLevelBGM(true);

            }
            else
            {
                AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);
            }
        };
    }
    private void PlayEndingCutscene()
    {
        CutscenesMenu cutscenesMenu = new(_cutScenes);
        cutscenesMenu.PlayEnding = () => {
            // Play the cutscene when its unlocked
            if (LevelManager.instance.arrLevels[LevelManager.instance.GetLevelIndexWithName("EndingCutscene")].Completed())
            // Play the cutscene when its unlocked
            {
                Debug.Log("Play Ending Cutscene");
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                vp.clip = endCutscene;
                vp.Play();
                _cutScenes.Display(false);
                GameManager.instance.GetLevelManager().PlayLevelBGM(true);

            }
            else
            {
                AudioManager.Instance.PlaySFX("menu_decline", Camera.main.transform.position);
            }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && vp.isPlaying)
        {
            CutsceneOver(vp);
        }
    }




    private void Quit()
    {
        Application.Quit();
    }
}
