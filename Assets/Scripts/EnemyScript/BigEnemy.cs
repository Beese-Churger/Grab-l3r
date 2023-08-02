using UnityEngine;


public class BigEnemy : EnemyBaseClass
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
    private float speed;
    private float stationaryTimer;
    private Rigidbody2D rb;
    //Current waypoint in the path
    //private Path path;
    //private Seeker seeker;
    //private int currentWayPoint;

    private Animator animator;
    private GameObject playerPrefab;
    private FSM current;
    private bool e_Alive;

    [SerializeField] LayerMask platformLayer;
    [SerializeField] LayerMask obstacleLayer;

    private float raycastDistance = 1f;
    public bool detected = false;
    //
    private int type = 1;
    private float direction = 1f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        // Set the current state of the enemy to patrol 
        current = FSM.PATROL;
        speed = originalSpeed;
        stationaryTimer = intervalBetweenPoints;
        e_Alive = true;


        //seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        rb.mass = weight * 0.10131712f;

        //InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);

    }
    //void UpdatePath()
    //{
    //    if (detected)
    //        seeker.StartPath(rb.position, playerPrefab.transform.position, OnPathComplete);            
    //}
    //void OnPathComplete(Path p)
    //{
    //    if (!p.error)
    //    {
    //        //Debug.Log("path completed");
    //        path = p;
    //        currentWayPoint = 0;
    //    }
    //}
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
                    if (!EdgeDetection() && !WallDetection())
                    {
                        Patrol();
                        Slow();
                        //AudioManager.Instance.PlaySFX("enemy_movement", transform.position);
                    }
                    break;
                case FSM.AGGRESSIVE:
                    // TO DO:
                    Follow();
                    // Debug.Log("Triggered!!!");
                    // If enemy touches the player, the player will instantly die                
                    //Vector2 force = speed * Time.deltaTime * dir;
                    //rb.AddForce(force);
                    Vector2 dir = ((Vector2)playerPrefab.transform.position - rb.position).normalized;
                    dir.y = 0;
                    direction = dir.x;
                    //Debug.Log(dir);

                    if (!EdgeDetection())
                    {
                        if (animator.gameObject.activeSelf)
                            animator.SetBool("Patrol", true);
                        transform.Translate(speed * Time.deltaTime * dir, Space.Self);
                    }
                    if (dir.x >= 0.01f)
                        transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
                    else if (dir.x <= -0.01f)
                        transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);
                    break;

            }
        }

    }
    public override int GetWeight()
    {
        return weight;
    }
    public override void SetWeight(int newWeight)
    {
        return;
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
        switch (stateNumber)
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
        if (col.gameObject.CompareTag("Player"))
        {
            current = FSM.AGGRESSIVE;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            current = FSM.IDLE;
            detected = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TO DO: SET THE PLAYER STATUS TO DEAD
        if (collision.gameObject.CompareTag("Player"))
            GameManager.instance.TakeDamage();
    }
    private void Patrol()
    {
        Vector2 dir = new(direction, 0);
        transform.Translate(speed * Time.deltaTime * dir, Space.Self);
        //Vector2 force = speed * Time.deltaTime * dir;
        //rb.AddForce(force);

        if (animator.gameObject.activeSelf)
            animator.SetBool("Patrol", true);
        if (direction >= 0.01f)
            transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        else if (direction <= -0.01f)
            transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);

    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        //float distanceFromDestination = Vector2.Distance(rb.position, waypoints[currentWP].transform.position);
        //if (distanceFromDestination < stoppingDistance)
        //{
        //    if (speed > 0)
        //    {
        //        speed = distanceFromDestination / stoppingDistance * originalSpeed;
        //    }
        //}
        //else
        //{
        //    speed = originalSpeed;
        //}
    }
    private bool EdgeDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, raycastDistance, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, raycastDistance, platformLayer);
        // Check if either of the raycasts hit a platform
        if ((leftHit.collider == null && direction < 0) || (rightHit.collider == null && direction > 0))
        {
            ChangeDirection();
            current = FSM.IDLE;
            //Debug.Log("Big Enemy is near the edge!");
            return true;
        }
        return false;
    }
    private bool WallDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, raycastDistance, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, raycastDistance, obstacleLayer);

       
        if ((leftHit.collider != null && direction < 0) || (rightHit.collider != null && direction > 0))
        {
            rb.velocity = Vector2.zero;
            ChangeDirection();
            current = FSM.IDLE;
            //Debug.Log("Big Enemy is near the wall!");      
            return true;
        }
        return false;
    }

    private void ChangeDirection()
    {
        speed = originalSpeed;
        // Change direction
        direction *= -1;
    }
}
