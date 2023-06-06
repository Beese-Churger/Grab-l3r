using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    // TODO: add more types of platforms 
    public enum TerrainType
    {
       concreate,
       vines,
       none
    }

    [SerializeField] TerrainType terrainType;
    private GameObject player;
    private bool triggerPressurePlate = false;
    public float speed = 2f;
    private Vector2 startPoint;
    public Vector2 endPoint;

    // Find player from scene
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPoint=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is grabbing true
        if(player.GetComponent<Player>().isGrabbing)
        {
            switch (terrainType)
            {
                case TerrainType.vines:
                    //attatch player
                    break;
                case TerrainType.concreate:
                    // detatch player 
                    player.GetComponent<Player>().isGrabbing = false;
                    break;
            }
            
        }

        if(triggerPressurePlate)
        {
            if(transform.position.x < endPoint.x)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPoint, speed*Time.deltaTime);
            }
            else
            {
                transform.position=Vector2.MoveTowards(transform.position, startPoint, speed*Time.deltaTime);
            }
        }

        
    }

    public void ActivateMovingPlatform(){
        triggerPressurePlate = true;
        Debug.Log(triggerPressurePlate);
    }
}
