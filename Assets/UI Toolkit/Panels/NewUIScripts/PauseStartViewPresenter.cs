using UnityEngine;
using UnityEngine.UIElements;

public class PauseStartViewPresenter : MonoBehaviour
{
    private VisualElement _pauseView;
    private VisualElement _optionsView;
    public VisualElement _volumeScreen;
    public VisualElement _brightnessScreen;
    public VisualElement _resolutionScreen;
    public VisualElement _controlScreen;



    private bool paused = false;

    private void Start()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        _pauseView = visualElement.Q("PauseScreen");
        _optionsView = visualElement.Q("OptionsMenu");

        _volumeScreen = visualElement.Q("VolumeMenu");
        _brightnessScreen = visualElement.Q("BrightnessMenu");
        _resolutionScreen = visualElement.Q("ResolutionDropdownMenu");
        _controlScreen = visualElement.Q("ControlsMenu");

        _pauseView.Display(false);

        Continue();
        Options();
        Quit();

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
    public void TogglePauseScreen(bool enable)
    {
        _pauseView.Display(enable);
        if (enable == false)
        {
            _optionsView.Display(enable);
            _volumeScreen.Display(enable);
            _brightnessScreen.Display(enable);
            _resolutionScreen.Display(enable);
            _controlScreen.Display(enable);
        }
        //Debug.Log("Paused");
    }

    // True - On Pause Screen, False - Off Pause Screen
    public void SwitchScreen(bool enable, VisualElement vE)
    {
        _pauseView.Display(enable);
        vE.Display(!enable);
    }
    // True -  first element Off && 2nd element On
    public void SwitchScreen(bool enable, VisualElement off, VisualElement on)
    {
        off.Display(!enable);
        on.Display(enable);
    }
    public void Continue()
    {      
        PauseMenuViewPresenter pauseMenuViewPresenter = new(_pauseView);
        pauseMenuViewPresenter.ContinueGame = () => {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            Debug.Log("continue");
            NewOptions.instance.SetPauseState(false);
            NewOptions.instance.SetPlayerInput("Gameplay");
            Time.timeScale = 1;
            TogglePauseScreen(false);
            paused = false;

        };

    }
    public void Options()
    {
        PauseMenuViewPresenter pauseMenuViewPresenter = new(_pauseView);
        pauseMenuViewPresenter.OpenOptions = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(false, _optionsView);
        };
    }
    public void Quit()
    {
        PauseMenuViewPresenter pauseMenuViewPresenter = new(_pauseView);
        pauseMenuViewPresenter.QuitGame = () =>
            {
                AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
                //LevelManager.instance.SetCurrentLevelIndex(-1);
                GameManager.instance.SetGameState(StateType.open);
                TogglePauseScreen(false);
                Time.timeScale = 1;
                NewOptions.instance.SetPlayerInput("Gameplay");
            };
    }
    private void CloseOptionsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        optionsMenuViewPresenter.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(true, _optionsView);
        };
    }
    private void OpenVolumeMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        optionsMenuViewPresenter.OpenVolume = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(true, _optionsView, _volumeScreen);
        };
    }
    private void CloseVolumeMenu()
    {
        VolumeMenu volumeMenu = new(_volumeScreen);
        volumeMenu.BackAction = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(false, _optionsView, _volumeScreen);
        };
    }
    private void OpenBrightnessMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        optionsMenuViewPresenter.OpenBrightness = () =>
        {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(true, _optionsView, _brightnessScreen);
        };
    }
    private void CloseBrightnessMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        BrightnessMenu brightnessMenu = new(_brightnessScreen);
        brightnessMenu.BackAction = () => { 
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(false, _optionsView, _brightnessScreen);
        };
    }
    private void OpenResolutionMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        optionsMenuViewPresenter.OpenResolution = () => { 
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position); 
            SwitchScreen(true, _optionsView, _resolutionScreen); 
        };
    }
    private void CloseResolutionMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        ResolutionMenu resolutionMenu = new(_resolutionScreen);
        resolutionMenu.BackAction = () => { 
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(false, _optionsView, _resolutionScreen);
        };
    }
    private void OpenControlsMenu()
    {
        OptionsMenuViewPresenter optionsMenuViewPresenter = new(_optionsView);
        optionsMenuViewPresenter.OpenControls = () => {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(true, _optionsView, _controlScreen);
        };
    }
    private void CloseControlsMenu()
    {
        ControlsMenu controlsMenu = new(_controlScreen);
        controlsMenu.BackAction = () => {
            AudioManager.Instance.PlaySFX("menu_confirm" + Random.Range(1, 2), Camera.main.transform.position);
            SwitchScreen(false, _optionsView, _controlScreen); 
        };
    }

    private void Update()
    {
        if (NewOptions.instance.GetPauseState() && !paused)
        {
            TogglePauseScreen(true);
            paused = true;
        }
        else if (!NewOptions.instance.GetPauseState() && paused)
        {
            TogglePauseScreen(false);
            paused = false;
        }
    }
}
