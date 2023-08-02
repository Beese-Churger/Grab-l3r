using UnityEngine.UIElements;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class HighscoreListViewPresenter : MonoBehaviour
{
    private Button _backButton;
    private ListView _highScoreList;
    private VisualTreeAsset vs;

    public class HighScoreData
    {
        public string playerName;
        public string time;
    }
    public Action BackAction { set => _backButton.clicked += value; }

    public HighscoreListViewPresenter(VisualElement root, VisualTreeAsset visualTreeAsset)
    {
        vs = visualTreeAsset;
        _backButton = root.Q<Button>("BackButton");
        _highScoreList = root.Q<ListView>("TableListView");
        _highScoreList.style.flexGrow = 1.0f;
        LoadHighScoreData();

    }
    private void LoadHighScoreData()
    {
        // Replace this example data with your actual high score data retrieval logic
        List<HighScoreData> highScores = new();
        //GameManager.instance.highscore = PlayerPrefs.GetInt("highscore");
        var temp = PlayerDataManager.instance.playerDataDictionary;
        for (int i = 0; i < temp.Count; ++i)
        {
            highScores.Add(new HighScoreData { playerName = temp[i].playerName , time = formatTimer(temp[i].timeTaken) });
        }
        var tempList = highScores.OrderBy(o=>o.time).ToList();
        PopulateListView(tempList);
    }

    private void PopulateListView(List<HighScoreData> highScores)
    {
        // Clear existing items in the ListView
        _highScoreList.Clear();

        // Add player data entries to the ListView
        foreach (var playerData in highScores)
        {
            var entry = vs.CloneTree();
            var entryPlayerNameLabel = entry.Q<Label>("PlayerNameLabel");
            var entryHighScoreLabel = entry.Q<Label>("HighScoreLabel");

            entryPlayerNameLabel.text = playerData.playerName;
            entryHighScoreLabel.text = playerData.time.ToString();

            _highScoreList.hierarchy.Add(entry);
        }
    }
    public string formatTimer(float currentTime)
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        return string.Format("{00:00} : {01:00}", minutes, seconds);
    }
}
