using UnityEngine;

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
    private PlayerController playerInstance;

    [SerializeField] LayerMask platformLayer;
    [SerializeField] LayerMask Layer;
    private float raycastDistance = 1f;
    public bool detected = false;
    RaycastHit2D empty;
    //
    private int type = 0;
    float direction = 1;
    bool onPPlate = false;
    bool isJumping = false;
    public bool isHooked = false;

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

    }

    public override void FSMUpdate()
    {
        if (IsGround())
        {
            if (isHooked)
                current = FSM.IDLE;
            
            switch (current)
            {
                case FSM.IDLE: // For IDLE State, The enemy temporarily stops moving before it starts moving again
                    if (animator.gameObject.activeSelf)
                        animator.SetBool("Patrol", false);
                    Stop();
                    break;
                case FSM.PATROL:
                    if (!EdgeDetection() && !ObstacleDetection())
                    {
                        Patrol();
                        //Slow();
                    }
                    break;
                case FSM.AGGRESSIVE:
                    // TO DO:
                    // If enemy touches the player, the player will instantly die      
                    Follow();

                    Vector2 dir = ((Vector2)playerPrefab.transform.position - rb.position).normalized;
                    dir.y = 0;
                    direction = dir.x;

                    if (!EdgeDetection())
                    {
                        Vector2 force = speed * Time.deltaTime * dir;
                        rb.AddForce(force);
                    }
                    if (dir.x >= 0.01f)
                        transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
                    else if (dir.x <= -0.01f)
                        transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);

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
        if (col.gameObject.CompareTag("Player"))
        {
            // TO DO: SET THE PLAYER STATUS TO DEAD
            if (playerInstance != null)
            {
                playerInstance.p_Alive = false;
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PressurePlate"))
        {
            Debug.Log(gameObject.name + " is on p plate");
            onPPlate = true;
        }
        else
            onPPlate = false;
    }
    private void Patrol()
    {
        Vector2 dir = new(direction, 0);
        Vector2 force = speed * Time.deltaTime * dir;
        rb.AddForce(force);

        if (animator.gameObject.activeSelf)
            animator.SetBool("Patrol", true);
        if (direction >= 0.01f)
            transform.localScale = new Vector3(spriteScale, spriteScale, 1f);
        else if (direction <= -0.01f)
            transform.localScale = new Vector3(-spriteScale, spriteScale, 1f);
    }
    private bool EdgeDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, raycastDistance, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, raycastDistance, platformLayer);

        // Check if either of the raycasts hit a platform
        if (((leftHit.collider == null && direction < 0) || (rightHit.collider == null && direction > 0)) && !onPPlate && !isJumping)
        {
            // Debug.Log(dir);
            rb.velocity = Vector2.zero;
            ChangeDirection();
            current = FSM.IDLE;
            //Debug.Log("Small Enemy is near the edge!");


            return true;
        }

        return false;

    }
    private bool ObstacleDetection()
    {
        // Cast two raycasts downward to check for nearby edges
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
        leftRayOrigin.y -= 0.1f;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;
        rightRayOrigin.y -= 0.1f;

        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, 0f, platformLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, 0f, platformLayer);

        if ((leftHit.collider != null && direction < 0) || (rightHit.collider != null && direction > 0))
        {
            CheckHit(leftHit, rightHit);
            Debug.Log(empty.transform.name);
            Debug.DrawRay(transform.position, (empty.transform.position - transform.position).normalized * 1f, Color.red);
            if (empty.transform.gameObject.layer == LayerMask.NameToLayer("PressurePlate"))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0, 1) * 5f, ForceMode2D.Impulse);
                rb.AddForce(new Vector2(direction, 0) * 3f, ForceMode2D.Impulse);
                isJumping = true;
                Debug.Log("Direction after jumping" + direction);
                return false;
            }
            
            rb.velocity = Vector2.zero;
            ChangeDirection();
            current = FSM.IDLE;
            isJumping = false;
            //Debug.Log("Big Enemy is near the wall!");  
            return true;
        }
        return false;
    }
    private void ChangeDirection()
    {
        // Change small enemy direction
        direction *= -1;
    }
    private void CheckHit(RaycastHit2D left, RaycastHit2D right)
    {
        if (left.collider != null)
            empty = left;
        else if (right.collider != null)
            empty = right;
    }
    private bool IsGround()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position + Vector3.down * 1f, Vector2.down, 1f, Layer);
        RaycastHit2D groundHit2 = Physics2D.Raycast(transform.position + Vector3.down + new Vector3(direction, 0, 0) * 1f, Vector2.down, 1f, Layer);
        if (groundHit.collider != null || groundHit2.collider != null)
        {
            //Debug.Log(groundHit.collider.name);
            if (groundHit.distance <= 0.02f)
            {
                //Debug.Log("Ground");
                return true;
            }
        }
        Debug.Log("InTheAir");
        return false;
              
    }

    // Slows big enemy movement down as it approaches the waypoint,
    // Not using at the moment
    //private void Slow()
    //{
    //    Vector3 leftRayOrigin = transform.position + Vector3.left * raycastDistance;
    //    Vector3 rightRayOrigin = transform.position + Vector3.right * raycastDistance;

    //    RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, 20f, platformLayer);
    //    RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, 20f, platformLayer);

    //    float distanceFromDestination = 0f;
    //    if (leftHit.collider != null && direction < 0)
    //    {
    //        distanceFromDestination = leftHit.distance;
    //    }
    //    else if (rightHit.collider != null && direction > 0)
    //    {
    //        distanceFromDestination = rightHit.distance;
    //    }

    //    if (distanceFromDestination < stoppingDistance)
    //    {
    //        speed *= 0.7f;
    //    }
    //    else
    //    {
    //        speed = originalSpeed;
    //    }

    //}

}
