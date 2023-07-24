using System;
using UnityEngine.UIElements;

public class PauseMenuViewPresenter
{
    private Button _continue;
    private Button _quit;

    public Action ContinueGame { set => _continue.clicked += value; }
    public Action QuitGame { set => _quit.clicked += value; }
    public PauseMenuViewPresenter(VisualElement root)
    {
        _continue = root.Q<Button>("ContinueButton");
        _quit = root.Q<Button>("PauseQuitButton");
    }

}
