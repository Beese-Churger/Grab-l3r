using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Vector2 checkpointPosition = new Vector2(-2,0);

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = checkpointPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
