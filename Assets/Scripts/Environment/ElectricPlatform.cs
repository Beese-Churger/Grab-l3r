using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1f;
    public float slowdownDistance = 1f;
    public bool moveOnStart = true;

    private Vector3 currentTarget;
    private float originalSpeed;
    private bool slowingDown;

    private bool isActive = false; 

    private void Start()
    {
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
        if (isActive)
        {
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
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.TakeDamage();
        }
    }
    public void SetActive()
    {
        isActive = !isActive;
    }
}
