using UnityEngine;
using UnityEngine.UIElements;

public class WinScreen : MonoBehaviour
{
    private Button backButton;
    private Label statsLabel;


    void Start()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        backButton = visualElement.Q<Button>("OKButton");
        statsLabel = visualElement.Q<Label>("TimeStamp");
        backButton.RegisterCallback<MouseUpEvent>((evt) => {
            GameManager.instance.SetGameState(StateType.end);
        });
        statsLabel.text = Timer.instance.time.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
