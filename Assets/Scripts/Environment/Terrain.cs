using UnityEngine;

public class Terrain : MonoBehaviour
{
    public bool triggerPressurePlate = false;
    public float speed = 2f;
    public Vector2 endPos;

    private Vector2 startPos;
    private GameObject player;

    public enum TerrainType
    {
       concreate,
       vines,
       moving
    }
    [SerializeField] TerrainType terrainType;
    

    // Find player from scene and set startposotion for moving platform
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = transform.position;
    }

    void Update()
    {
        //// check if player is grabbing platform
        //if (player.GetComponent<Player>().isGrabbing)
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

        // activate moving platform on pressure plate press
        if (triggerPressurePlate)
        {
            // move platfrom between start and target points
            transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f));
        }
    }

    // activate moving platform
    public void ActivateMovingPlatform(){
        triggerPressurePlate = true;
    }
}
