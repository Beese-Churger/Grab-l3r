using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    public enum EventType
    {
        gameEnd
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static GameManager getInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();
        }
        return instance;
    }
}
