using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public float timeTaken;

    public PlayerData(string playerName, float timeTaken)
    {
        this.playerName = playerName;
        this.timeTaken = timeTaken;
    }
}
