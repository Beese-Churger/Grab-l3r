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
    public Animator animator;

    private bool triggerPressurePlate = false;
    private Vector2 startPos;
    private GameObject player;

    void Start()
    {
        startPos = transform.position;
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
        animator.SetBool("isActivated", true);
    }

    public void DeactivateMovingPlatform()
    {
        triggerPressurePlate = false;
        animator.SetBool("isActivated", false);
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
}