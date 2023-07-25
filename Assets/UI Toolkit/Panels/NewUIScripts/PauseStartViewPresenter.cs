using UnityEngine;
using UnityEngine.UIElements;

public class PauseStartViewPresenter : MonoBehaviour
{
    private VisualElement _pauseView;

    private bool paused = false;

    private void Start()
    {
        _pauseView = GetComponent<UIDocument>().rootVisualElement;
        _pauseView.Display(false);
        Continue();
        Quit();
    }
    public void TogglePauseScreen(bool enable)
    {
        _pauseView.Display(enable);
        //Debug.Log("Paused");
    }
    public void Continue()
    {      
        PauseMenuViewPresenter pauseMenuViewPresenter = new(_pauseView);
        pauseMenuViewPresenter.ContinueGame = () => {
            Debug.Log("continue");
            NewOptions.instance.SetPauseState(false);
            paused = false;
        };

    }
    public void Quit()
    {
        PauseMenuViewPresenter pauseMenuViewPresenter = new(_pauseView);
        pauseMenuViewPresenter.QuitGame = () =>
            { 
                LevelManager.instance.SetCurrentLevelIndex(-1);
                GameManager.instance.SetGameState(StateType.open);
                TogglePauseScreen(false);
                Time.timeScale = 1;
                NewOptions.instance.SetPlayerInput("Gameplay");
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
