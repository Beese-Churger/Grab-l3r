using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayableCreditsView : MonoBehaviour
{
    // Start is called before the first frame update
    private Button _backButton;
    void Start()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        _backButton = visualElement.Q<Button>("BackButton");
        _backButton.RegisterCallback<MouseUpEvent>((evt) => {
             GameManager.instance.SetGameState(StateType.levelChange);
        });
        NewOptions.instance.SetPlayerInput("Options");

    }

}
