using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    // TODO: add more types of platforms 
    public enum TerrainType
    {
       concreate,
       vines
    }

    [SerializeField] TerrainType terrainType;
    private GameObject player;

    // Find player from scene
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
    }
}
