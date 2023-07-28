using UnityEngine.UIElements;
using System;
using UnityEngine;
using System.Collections.Generic;

public class HighscoreListViewPresenter
{
    private Button _backButton;
    private ListView _highScoreList;

    public class HighScoreData
    {
        public string playerName;
        public int score;
    }
    public Action BackAction { set => _backButton.clicked += value; }

    public HighscoreListViewPresenter(VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _highScoreList = root.Q<ListView>("ListView");
        _highScoreList.style.flexGrow = 1.0f;
        LoadHighScoreData();

    }
    private void LoadHighScoreData()
    {
        // Replace this example data with your actual high score data retrieval logic
        List<HighScoreData> highScores = new();
        //GameManager.instance.highscore = PlayerPrefs.GetInt("highscore");
        var hs = PlayerPrefs.GetInt("highscore");

        highScores.Add(new HighScoreData { playerName = "Player 1", score = hs });
        highScores.Add(new HighScoreData { playerName = "Player 2", score = 800 });
        highScores.Add(new HighScoreData { playerName = "Player 3", score = 600 });

        PopulateListView(highScores);
    }

    private void PopulateListView(List<HighScoreData> highScores)
    {
        // Clear existing items in the ListView
        _highScoreList.Clear();

        // Create and add new list items for each high score entry
        foreach (HighScoreData scoreData in highScores)
        {
            var listItem = new VisualElement();
            Label label1 = new Label(scoreData.playerName);
            label1.style.fontSize = 24;
            listItem.Add(label1);

            var listItem2 = new VisualElement();
            Label label2 = new Label(scoreData.score.ToString());
            label2.style.fontSize = 24;
            listItem2.Add(label2);

            _highScoreList.hierarchy.Add(listItem);
            _highScoreList.hierarchy.Add(listItem2);
        }
    }
}
