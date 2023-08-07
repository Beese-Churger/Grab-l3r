using UnityEngine;
using UnityEngine.UIElements;

public class WinScreen : MonoBehaviour
{
    private Button backButton;
    private Label statsLabel;
    private Label collectibleLabel;

    private VisualElement highscoreInput;
    private VisualElement background;
    void Start()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        background = visualElement.Q("Background");
        highscoreInput = visualElement.Q("HighscoreInput");
        backButton = visualElement.Q<Button>("OKButton");
        statsLabel = visualElement.Q<Label>("TimeStamp");
        collectibleLabel = visualElement.Q<Label>("CollectiblesStamp");

        backButton.RegisterCallback<MouseUpEvent>((evt) => {
            background.Display(false);
            highscoreInput.Display(true);
            //GameManager.instance.SetGameState(StateType.end);
        });
        ButtonsPressed();
        collectibleLabel.text += GameManager.instance.GetCollectables().ToString();
        statsLabel.text = Timer.instance.formatTimer(Timer.instance.time);
        

    }
    private void ButtonsPressed()
    {
        HighscoreInput high = new(highscoreInput);
        high.OkButtonDown = () => {
            string storeName = high.textField.text;
            if (!PlayerDataManager.instance.CheckForExistingName(storeName))
            {
                if (high.textField.text == "")
                    storeName = RandomNameGenerator.GenInstance.GetRandomName();
                PlayerDataManager.instance.SavePlayerData(storeName, Timer.instance.time);
                GameManager.instance.SetGameState(StateType.end);
            }
            else
                Debug.Log("Please Enter Another Name");

             
        };
        high.CancelButtonDown = () => {
            string storeName = high.textField.text;
            if (!PlayerDataManager.instance.CheckForExistingName(storeName))
            {
                if (high.textField.text == "")
                    storeName = RandomNameGenerator.GenInstance.GetRandomName();
                PlayerDataManager.instance.SavePlayerData(storeName, Timer.instance.time);
                GameManager.instance.SetGameState(StateType.end);
            }
            else
                Debug.Log("Please Enter Another Name");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
