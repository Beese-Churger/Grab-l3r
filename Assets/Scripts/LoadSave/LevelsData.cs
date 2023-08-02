using System.Collections.Generic;

[System.Serializable]
public class LevelsData
{
    public List<Level> levels;

    public LevelsData(List<Level> levels)
    {
        this.levels = levels;
    }
}
