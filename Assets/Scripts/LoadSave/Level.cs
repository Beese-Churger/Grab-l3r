using UnityEngine;

[System.Serializable]
public class Level
{
    public int level { get; set; }
    public bool isCompleted { get; set; }

    public Level(int level, bool isCompleted)
    {
        this.level = level;
        this.isCompleted = isCompleted;
    }

    public int GetIndex()
    {
        return level;
    }

    public bool Completed()
    {
        return isCompleted;
    }
}
