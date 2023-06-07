using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float intervalBetweenPoints;
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1f;
    public float slowdownDistance = 1f;
    public bool moveOnStart = true;

    private Vector3 currentTarget;
    private float originalSpeed;
    private bool slowingDown;

    private float interval;
    private bool reach = false;

    private void Start()
    {
        // Set the initial target to the start point
        currentTarget = startPoint.position;

        interval = intervalBetweenPoints;

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
        if (!reach)
        {
            // Calculate the distance to the target position
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget);

            // Check if the platform has reached the current target position
            if (distanceToTarget <= slowdownDistance && !slowingDown)
            {
                slowingDown = true;
                //Debug.Log(speed);
            }
            if (slowingDown)
                if (speed > 5)
                    speed = distanceToTarget / slowdownDistance * originalSpeed;


            // Move towards the current target position
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

            // Check if the platform has reached the current target position
            if (transform.position == currentTarget)
            {
                slowingDown = false;
                reach = true;
                speed = originalSpeed;

                // Switch the target position
                if (currentTarget == startPoint.position)
                    currentTarget = endPoint.position;
                else
                    currentTarget = startPoint.position;
            }
        }
        else
        {
            if (interval > 0)
            {
                interval -= Time.deltaTime;
            }
            else
            {
                interval = intervalBetweenPoints;
                reach = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a line in the Scene view to visualize the platform's movement path
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
}
