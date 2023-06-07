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
       moving
    }

    [SerializeField] TerrainType terrainType;
    private GameObject player;
    public bool triggerPressurePlate = false;
    public float speed = 2f;
    private Vector2 startPos;
    public Vector2 endPos;
    private bool isRight;

    // Find player from scene
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //// check if player is grabbing true
        //if(player.GetComponent<Player>().isGrabbing)
        //{
        //    switch (terrainType)
        //    {
        //        case TerrainType.vines:
        //            //attatch player
        //            break;
        //        case TerrainType.concreate:
        //            // detatch player 
        //            player.GetComponent<Player>().isGrabbing = false;
        //            break;
        //    }
            
        //}

        if(triggerPressurePlate)
        {
            transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f));
        }
    }

    public void ActivateMovingPlatform(){
        triggerPressurePlate = true;
        Debug.Log(triggerPressurePlate);
    }
}
