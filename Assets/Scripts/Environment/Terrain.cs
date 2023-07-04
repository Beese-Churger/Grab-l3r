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

    public float speed = 0.5f;
    public Vector2 endPos;
    public Sprite newSprite;

    private bool triggerPressurePlate = false;
    private SpriteRenderer spriteRender;
    private Sprite mySprite;
    private Vector2 startPos;
    private GameObject player;

    void Start()
    {
        startPos = transform.position;
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRender != null)
        {
            mySprite = spriteRender.sprite;
        }
        player = GameObject.Find("Player");
    }

    void Update()
    {
        switch (terrainType)
        {
            case TerrainType.moving:
                if (triggerPressurePlate)
                {
                    transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f));
                }
                break;
        }
    }

    public void ActivateMovingPlatform()
    {
        triggerPressurePlate = true;
        spriteRender.sprite = newSprite;
    }

    public void DeactivateMovingPlatform()
    {
        triggerPressurePlate = false;
        spriteRender.sprite = mySprite;
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
}