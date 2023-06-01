using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Vector2 checkpointPosition = new Vector2(-2,0);
    public bool isGrabbing = false;
    
    private void Awake()
    {
        // set player position to last checkpoint on respawn
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check collision with terrain
        if (collision.transform.tag == "Terrain")
        {
            // TODO: check terrain surface type
        }
    }
}
