using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{    
    public void TriggerStart()
    {
        GameManager.instance.SetGameState(StateType.levelChange);
    }
    public void TriggerExit()
    {
        Application.Quit();
    }

}
