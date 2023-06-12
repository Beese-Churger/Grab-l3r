using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Temporary until GameManager Works
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void TriggerStart()
    {
        // TEMPORARY until GameManager works

        //Debug.Log("Changed Level");
        SceneManager.LoadScene("Gameplay");
        //GameManager.instance.SetGameState(StateType.levelChange);
    }
    public void TriggerExit()
    {
        Application.Quit();
    }

}
