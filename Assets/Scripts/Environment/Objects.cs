using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public List<GameObject> objects = new();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
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
