using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMainMenuPresenter : MonoBehaviour
{
    private void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("NewGameButton").clicked += () => Debug.Log("New Game Button clicked");
        root.Q<Button>("LoadGameButton").clicked += () => Debug.Log("Load Game Button clicked");
        root.Q<Button>("HighscoreButton").clicked += () => Debug.Log("Highscore Button clicked");
        root.Q<Button>("CutscenesButton").clicked += () => Debug.Log("Cutscenes Button clicked");
        root.Q<Button>("OptionsButton").clicked += () => Debug.Log("Options Button clicked");
        root.Q<Button>("CreditsButton").clicked += () => Debug.Log("Credits Button clicked");
        root.Q<Button>("QuitButton").clicked += () => Debug.Log("Quit Button clicked");
    }
}
