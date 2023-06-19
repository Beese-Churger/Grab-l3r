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
    public GameObject player;

    private Vector2 startPos;
    private RopeSysTest ropeScript;

    // Find player from scene and set startposotion for moving platform
    void Start()
    {
        startPos = transform.position;
        ropeScript = player.GetComponent<RopeSysTest>();
    }

    void Update()
    {
        switch (terrainType)
        {
            case TerrainType.concreate:
                break;
            case TerrainType.vines:
                break;
            case TerrainType.moving:
                // activate moving platform on pressure plate press
                if (triggerPressurePlate)
                {
                    // move platfrom between start and target points
                    transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f));
                }
                break;
        }
    }

    // activate moving platform
    public void ActivateMovingPlatform()
    {
        triggerPressurePlate = true;
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
}