using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    // TODO: add more types of terrain (pressure plates etc...)
    public enum TerrainType
    {
        grabbable,
        slippery,
        pressure,
        button
    }

    [SerializeField] TerrainType terrainType;
    private GameObject player;

    // Start is called before the first frame update
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
                case TerrainType.grabbable:
                    //attatch player?
                    break;
                case TerrainType.slippery:
                    // detatch player
                    break;
            }
        }
    }
}
