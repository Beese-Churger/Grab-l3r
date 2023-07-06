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
    private float timer;
    private float delay = 2f;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Awake()
    {
        isRight = false;
    }

    void Update()
    {
        switch (terrainType)
        {
            case TerrainType.moving:
                if(triggerPressurePlate)
                {
                    Move();
                }
                break;
        }
    }

    private void Move()
    {
        if (!isRight)
        {
            if (Vector2.Distance(transform.position, endPos) < 0.001f)
            {
                transform.position = endPos;
                timer += Time.deltaTime;
                if(timer > delay)
                {
                    isRight = true;
                    timer = 0;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, startPos) < 0.001f)
            {
                transform.position = startPos;
                timer += Time.deltaTime;
                if (timer > delay)
                {
                    isRight = false;
                    timer = 0;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            }
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