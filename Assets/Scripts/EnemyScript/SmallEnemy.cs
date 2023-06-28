using UnityEngine;
using Pathfinding;
using System;
public class SmallEnemy : EnemyBaseClass
{
    enum FSM
    {
        IDLE,
        PATROL,
        AGGRESSIVE,
        DEAD,
        NUM_STATE
    };
    [SerializeField] private int weight;
    
    // Enemy Patrol Variables
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float intervalBetweenPoints;
    [SerializeField] private float patrolDistance = 10.0f;
    [SerializeField] private float spriteScale = 1f;

    private GameObject[] waypoints;
    private GameObject wayPointObject;
    private int currentWP;
    private float speed;
    private float stationaryTimer;
    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;
    //Current waypoint in the path
    private int currentWayPoint;

    private Animator animator;
    private GameObject playerPrefab;
    private FSM current;
    private bool e_Alive;
    private PlayerController playerInstance;

    [SerializeField] LayerMask platformLayer;
    private float raycastDistance = 1f;
    public bool detected = false;
    //
    private int type = 0;

    float y, prevY;
    bool isGrounded = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        wayPointObject = GameObject.Find("EnemyWaypoints");

        waypoints = new GameObject[2];
        waypoints[0] = new GameObject("SmallEnemyWaypoint");
        waypoints[1] = new GameObject("SmallEnemyWaypoint");

        waypoints[0].transform.SetParent(wayPointObject.transform);
        waypoints[1].transform.SetParent(wayPointObject.transform);
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
        //if (!detected)
        //    seeker.StartPath(rb.position, waypoints[currentWP].transform.position, OnPathComplete);
        //else
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
        //if (e_Alive)
        {
            switch (current)
            {
                case FSM.IDLE: // For IDLE State, The enemy temporarily stops moving before it starts moving again
                    if (animator.gameObject.activeSelf)
                        animator.SetBool("Patrol", false);
                    Stop();
                    break;
                case FSM.PATROL:
                    if (Math.Abs(waypoints[currentWP].transform.position.x - rb.position.x) <= 1f)
                    {
                        // Change patrol points
                        current = FSM.IDLE;
                        CheckCurrentWP();
                        return;
                    }

                    if (!EdgeDetection() && !WallDetection())
                    {
                        Patrol();
                        if (speed == originalSpeed)
                            Slow();
                    }

                    break;
                case FSM.AGGRESSIVE:
                    // TO DO:
                    if (path == null)
                    {
                        Debug.Log("No path found");
                        return;
                    }
                    if (currentWayPoint >= path.vectorPath.Count)
                    {
                        speed = originalSpeed;
                        current = FSM.IDLE;
                        Debug.Log("CWP:" + currentWayPoint + "pathcount:" + path.vectorPath.Count);

                        return;
                    }
                    // If enemy touches the player, the player will instantly die
                    Follow();
                    Following();
                    break;
                case FSM.DEAD:
                    break;

            }
        }

    }
    public override void SetWeight(int newWeight)
    {
        if (newWeight <= 15)
        {
            weight = newWeight;
            rb.mass = weight * 0.10131712f;
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
            speed = originalSpeed;
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
    private void Following()
    {
        Vector2 dir = ((Vector2)playerPrefab.transform.position - rb.position).normalized;
        Vector2 force = speed * Time.deltaTime * dir;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < playerPrefab.transform.localScale.x)
        {
            currentWayPoint++;
        }
        if (force.x >= 0.01f)
            transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        else if (force.x <= -0.01f)
            transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);
    }
    public void SetState(int stateNumber)
    {
        switch(stateNumber)
        {
            case (int)FSM.IDLE:
            current = FSM.IDLE;
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
        Vector2 dir = ((Vector2)waypoints[currentWP].transform.position - rb.position).normalized;
        dir.y = 0;
        Vector2 force = speed * Time.deltaTime * dir;
        rb.AddForce(force);

        if (animator.gameObject.activeSelf)
            animator.SetBool("Patrol", true);
        if (force.x >= 0.01f)
            transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        else if (force.x <= -0.01f)
            transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);
    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        float distanceFromDestination = Vector2.Distance(rb.position, waypoints[currentWP].transform.position);
        {
            if (distanceFromDestination < stoppingDistance)
            {
                speed *= 0.7f;
            }
            else
            {
                speed = originalSpeed;
            }
        }

    }
    private bool EdgeDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, raycastDistance, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, raycastDistance, platformLayer);

        //Debug.Log(rb.velocity.x);
        Vector2 dir = (Vector2)waypoints[currentWP].transform.position - rb.position;
        
        // Check if either of the raycasts hit a platform
        if ((leftHit.collider == null && dir.x < 0)  || (rightHit.collider == null && dir.x > 0))
        {
            // Debug.Log(dir);
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            CheckCurrentWP();
            current = FSM.IDLE;
            //Debug.Log("Small Enemy is near the edge!");
            return true;
        }
        return false;

    }
    private bool WallDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, 0f, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, 0f, platformLayer);
        if ((leftHit.collider != null && rb.velocity.x < 0) || (rightHit.collider != null && rb.velocity.x > 0))
        {
            rb.velocity = Vector2.zero;
            CheckCurrentWP();
            current = FSM.IDLE;
            //Debug.Log("Small Enemy is near the wall!");
            return true;
        }
        return false;
    }
    private void CheckCurrentWP()
    {
        // Change patrol points
        if (currentWP == 0)
            currentWP = 1;
        else
            currentWP = 0;
    }

}
