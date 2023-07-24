using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public List<GameObject> objects = new();

    // check that all important objects still exists
    void Update()
    {
        for(int i = 0; i < objects.Count; i++)
        {
            if(objects[i] == null)
            {
                Debug.Log("Important Object destroyed!");
                GameManager.instance.SetGameState(StateType.respawn);
            }
        }
    }
}
