using UnityEngine;

public class Terrain : MonoBehaviour
{
    public enum TerrainType
    {
        concreate,
        vines,
        moving
    }
    [SerializeField] TerrainType terrainType;

    public float speed = 2f;
    public Vector2 endPos;
    public bool triggerPressurePlate = false;
    public Obstacle obstacle;

    private Vector2 startPos;

    // Find player from scene and set startposotion for moving platform
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        //switch (terrainType)
        //{
        //    case TerrainType.concreate:
        //        ResetRope()
        //            break;
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
