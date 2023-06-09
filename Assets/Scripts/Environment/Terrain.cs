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

        // activate moving platform
        if(triggerPressurePlate)
        {
            transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f));
        }
    }

    // call function to activate moving platform
    public void ActivateMovingPlatform(){
        triggerPressurePlate = true;
    }

    // attatch player to platform transform when attatched
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

}
