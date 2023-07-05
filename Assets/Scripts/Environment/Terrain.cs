using UnityEngine;
using UnityEngine.InputSystem;

public class Terrain : MonoBehaviour
{
    public enum TerrainType
    {
        concreate,
        vines,
        moving
    }
    [SerializeField] TerrainType terrainType;

    public float speed = 0.2f;
    public Vector2 endPos;
    public Animator animator;

    public bool triggerPressurePlate = false;
    private Vector2 startPos;
    private bool isRight;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        switch (terrainType)
        {
            case TerrainType.moving:
                if (triggerPressurePlate)
                {
                    //transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, 1f))
                    transform.position=Vector2.MoveTowards(transform.position, endPos, Time.deltaTime*speed);
                }
                break;
        }
    }

    //private void MoveLeft()
    //{
    //    var step = speed * Time.deltaTime;
    //    transform.position=Vector2.MoveTowards(transform.position, startPos, step);
    //}

    //private void MoveRight()
    //{
    //    var step=speed* Time.deltaTime;
    //    transform.position=Vector2.MoveTowards(transform.position, endPos, step);
    //}

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