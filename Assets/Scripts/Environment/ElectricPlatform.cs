using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    [SerializeField] Animator elecAnimator;
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1f;
    public float slowdownDistance = 1f;
    public bool moveOnStart = true;

    private Vector3 currentTarget;
    private float originalSpeed;
    private bool slowingDown;

    private bool isActive = false;
    // temp fix
    public bool activate = true;

    private Terrain terrainComponent;
    private Obstacle obstacleComponent;
    private void Start()
    {
        terrainComponent = GetComponent<Terrain>();
        obstacleComponent = GetComponent<Obstacle>();
        obstacleComponent.DisableObstacle();
        // Set the initial target to the start point
        currentTarget = startPoint.position;

        if (moveOnStart)
        {
            // Start moving towards the end point
            transform.position = startPoint.position;
            currentTarget = endPoint.position;
        }

        originalSpeed = speed;
    }

    private void Update()
    {
        if (isActive && activate)
        {
            terrainComponent.SetTerrainType(Terrain.TerrainType.concreate);
            obstacleComponent.ActivateObstacle();
            elecAnimator.SetBool("Active", true);
            // Calculate the distance to the target position
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget);

            // Check if the platform has reached the current target position
            if (distanceToTarget <= slowdownDistance && !slowingDown)
            {
                slowingDown = true;
                speed = distanceToTarget / slowdownDistance;
            }

            // Move towards the current target position
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

            // Check if the platform has reached the current target position
            if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
            {
                slowingDown = false;

                // Switch the target position
                if (currentTarget == startPoint.position)
                    currentTarget = endPoint.position;
                else
                    currentTarget = startPoint.position;
                speed = originalSpeed;
            }
        }
        else
        {
            terrainComponent.SetTerrainType(Terrain.TerrainType.moving);
            obstacleComponent.DisableObstacle();
            elecAnimator.SetBool("Active", false);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
