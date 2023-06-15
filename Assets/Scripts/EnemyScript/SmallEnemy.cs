using UnityEngine;
using Unity.Mathematics;
using Pathfinding;

public class SmallEnemy : EnemyBaseClass
{
    enum FSM
    {
        NEUTRAL,
        PATROL,
        AGGRESSIVE,
        DEAD,
        NUM_STATE
    };
    private GameObject[] waypoints;
    [SerializeField] private int weight;
    
    // Enemy Patrol Variables
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float intervalBetweenPoints;
    [SerializeField] private float patrolDistance = 10.0f;
    private int currentWP;
    private float speed;
    private float stationaryTimer;
    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;
    //Current waypoint in the path
    private int currentWayPoint;


    private GameObject playerPrefab;
    private FSM current;
    private bool e_Alive;
    private PlayerController playerInstance;

    [SerializeField] LayerMask platformLayer;
    private float raycastDistance = 1f;
    private bool isNearEdge = false;
    public bool detected = false;
    //
    private int type = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");


        waypoints = new GameObject[2];
        waypoints[0] = new GameObject("Waypoint");
        waypoints[1] = new GameObject("Waypoint");
        // Set the current state of the enemy to patrol 
        current = FSM.PATROL;
        speed = originalSpeed;
        stationaryTimer = intervalBetweenPoints;
        e_Alive = true;

        waypoints[0].transform.position = transform.position - new Vector3(patrolDistance, 0, 0);
        waypoints[1].transform.position = transform.position + new Vector3(patrolDistance, 0, 0);

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        rb.mass = weight * 0.10131712f;

        InvokeRepeating("UpdatePath", 0f, 0.5f);


    }
    void UpdatePath()
    {
        if (!detected)
            seeker.StartPath(rb.position,waypoints[currentWP].transform.position, OnPathComplete);
        else
            seeker.StartPath(rb.position, playerPrefab.transform.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }
    public override void FSMUpdate()
    {
        // Might change this implementation if there is a DEAD state
        if (e_Alive)
        {
            switch (current)
            {
                case FSM.NEUTRAL: // For NEUTRAL State, The enemy temporarily stops moving before it starts moving again
                    Stop();
                    break;
                case FSM.PATROL:
                    if (path == null)
                        return;
                    // For PATROL State, The enemy would be patrolling around it's own platform to find the player

                    // Check if enemy has reached it's final destination
                    if (currentWayPoint >= path.vectorPath.Count)
                    {
                        // Change patrol points
                        if (currentWP == 0)
                            currentWP = 1;
                        else
                            currentWP = 0;
                        Stop();
                        return;
                    }

                    EdgeDetection();

                    if (!isNearEdge)
                    {
                        Patrol();
                        Slow();
                    }

                    break;
                case FSM.AGGRESSIVE:
                    // TO DO:
                    if (path == null)
                        return;
                    if (currentWayPoint >= path.vectorPath.Count)
                    {
                        Stop();
                        speed = originalSpeed;
                        return;
                    }
                    // Debug.Log("Triggered!!!");
                    // If enemy touches the player, the player will instantly die
                    Follow();
                    Vector2 dir = ((Vector2)playerPrefab.transform.position - rb.position).normalized;
                    Vector2 force = speed * Time.deltaTime * dir;

                    rb.AddForce(force);

                    float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

                    if (distance < playerPrefab.transform.localScale.x)
                    {
                        currentWayPoint++;
                    }
                    if (force.x >= 0.01f)
                        transform.localScale = new Vector3(-1f, 1f, 1f);
                    else if (force.x <= -0.01f)
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    break;

            }
        }

    }
    public override int GetWeight()
    {
        return weight;
    }

    public override void SetStatus(bool b_Status)
    {
        e_Alive = b_Status;
    }
    public override bool GetStatus()
    {
        return e_Alive;
    }
    public override int GetEnemyType()
    {
        return type;
    }
    /* Make the big enemy stationary for 1 second before allowing it to move to
       the next waypoint
     */
    private void Stop()
    {
        // Timer for the one second interval between waypoints
        if (stationaryTimer > 0)
        {
            stationaryTimer -= Time.deltaTime;
        }
        else
        {
            stationaryTimer = intervalBetweenPoints;
            current = FSM.PATROL;
            //Debug.Log("Timer over moving to wp" + currentWP);
        }
    }
    /* Follows the player
      */
    private void Follow()
    {
        if (detected)
            return;
        detected = true;
        speed = chaseSpeed;
    }
    public void SetState(int stateNumber)
    {
        switch(stateNumber)
        {
            case (int)FSM.NEUTRAL:
            current = FSM.NEUTRAL;
            break;
            case (int)FSM.PATROL:
            current = FSM.PATROL;
            break;
            case (int)FSM.AGGRESSIVE:
            current = FSM.AGGRESSIVE;
            break;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            // TO DO: SET THE PLAYER STATUS TO DEAD
            if (playerInstance != null)
            {
                playerInstance.p_Alive = false;
            }

        }
    }
    private void Patrol()
    {
        Vector2 dir = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = speed * Time.deltaTime * dir;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < stoppingDistance)
        {
            currentWayPoint++;
        }
        if (force.x >= 0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (force.x <= -0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        //Vector3 dir = (waypoints[currentWP].transform.position - transform.position).normalized;
        //transform.position += dir * speed * Time.deltaTime;
        //transform.position = Vector2.Lerp(transform.position, waypoints[currentWP].transform.position, Time.deltaTime);
        //Debug.Log("ENEMY POSITION: " + gameObject.transform.position);
        //Debug.Log("WAYPOINT POSITION: " + waypoints[currentWP].transform.position);
    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        float distanceFromDestination = Vector2.Distance(rb.position, waypoints[currentWP].transform.position);
        if (distanceFromDestination < stoppingDistance)
        {
            if (speed > 0)
            {
                speed = distanceFromDestination / stoppingDistance * originalSpeed;
            }
        }
        else
        {
            speed = originalSpeed;
        }
    }
    private void EdgeDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, raycastDistance, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, raycastDistance, platformLayer);

        //Debug.Log(rb.velocity.x);

        // Check if either of the raycasts hit a platform
        if ((leftHit.collider == null && rb.velocity.x < 0)  || (rightHit.collider == null && rb.velocity.x > 0))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            isNearEdge = true;
            if (currentWP == 0)
                currentWP = 1;
            else
                currentWP = 0;
            current = FSM.NEUTRAL;
            Debug.Log("Object is near the edge!");
        }
        else
        {
            isNearEdge = false;
        }
    }
   

}
