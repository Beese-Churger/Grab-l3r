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
    private GameObject player;
    public Sprite newSprite;

    private Sprite mySprite;
    private Vector2 startPos;
    private SpriteRenderer spriteRender;

    // Find player from scene and set startposotion for moving platform
    void Start()
    {
        startPos = transform.position;
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRender != null)
            mySprite = spriteRender.sprite;
        player = GameObject.Find("Player");
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
        spriteRender.color = Color.cyan;
        spriteRender.sprite = newSprite;
    }

    public void DeactivateMovingPlatform()
    {
        triggerPressurePlate = false;
        spriteRender.color = Color.white;
        spriteRender.sprite = mySprite;
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
}