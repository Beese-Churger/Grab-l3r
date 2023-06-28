using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    public void TriggerStart()
    {
        // TEMPORARY until GameManager works

        //Debug.Log("Changed Level");
        //GameManager.instance.SetGameState(StateType.boss);
        GameManager.instance.SetGameState(StateType.levelChange);
    }
    public void TriggerExit()
    {
        Application.Quit();
    }

}
