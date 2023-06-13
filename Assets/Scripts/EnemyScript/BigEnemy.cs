using UnityEngine;
using Unity.Mathematics;

public class BigEnemy : EnemyBaseClass
{
    enum FSM
    {
        NEUTRAL,
        PATROL,
        AGGRESSIVE,
        NUM_STATE
    };
    private GameObject[] waypoints;
    [SerializeField] private int weight;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float intervalBetweenPoints;
    [SerializeField] private float patrolDistance = 5.0f;



    private bool e_Alive;
    private GameObject playerPrefab;
    private FSM current;
    private int currentWP;
    private float speed;
    private float stationaryTimer;
    private float rotation;
    private PlayerController playerInstance;
    bool isGrounded = false;

    //
    private int type = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        waypoints = new GameObject[2];
        waypoints[0] = new GameObject();
        waypoints[1] = new GameObject();

        // Set current state to patrol mode
        current = FSM.PATROL;
        currentWP = 0;
        speed = 25;
        stationaryTimer = intervalBetweenPoints;
        rotation = 180;
        transform.localRotation = Quaternion.Euler(0, rotation, 0);
        e_Alive = true;

        // Set the position of the enemy at the start of the first waypoint
        //transform.position = waypoints[currentWP].transform.position;

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
                    // For PATROL State, The enemy would be patrolling around it's own platform to find the player
                    if (math.distance(transform.position.x, waypoints[0].transform.position.x) < 0.05 && currentWP == 0)
                    {
                        currentWP = 1;
                        current = FSM.NEUTRAL;
                    }
                    if (math.distance(transform.position.x, waypoints[1].transform.position.x) < 0.05 && currentWP == 1)
                    {
                        currentWP = 0;
                        current = FSM.NEUTRAL;

                    }
                    Patrol();
                    Slow();
                    break;
                case FSM.AGGRESSIVE:
                    // TO DO:
                    Debug.Log("Triggered!!!");
                    // If enemy touches the player, the player will instantly die
                    Follow();
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
    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWP].transform.position, speed * Time.deltaTime);
        //transform.position = Vector2.Lerp(transform.position, waypoints[currentWP].transform.position, Time.deltaTime);
        //Debug.Log("ENEMY POSITION: " + gameObject.transform.position);
        //Debug.Log("WAYPOINT POSITION: " + waypoints[currentWP].transform.position);
        //Debug.Log("Big Enemy Going Towards WP "+ currentWP);
    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        float distanceFromDestination = Vector2.Distance(transform.position, waypoints[currentWP].transform.position);

        if (distanceFromDestination < stoppingDistance)
        {
            if (speed > 1.5)
            {
                speed = distanceFromDestination / stoppingDistance * originalSpeed;
            }
        }
        else
        {
            speed = originalSpeed;
        }
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
            // Flips FOV
            rotation -= 180;
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
            current = FSM.PATROL;
            //Debug.Log("Timer over moving to wp" + currentWP);
        }
    }
    /* Follows the player
      */
    private void Follow()
    {
        {
            Vector3 dir = (playerPrefab.transform.position - transform.position).normalized;
            dir.y = 0;
            transform.position += dir * speed * Time.deltaTime;
        }
    }
    public void SetState(int stateNumber)
    {
        switch (stateNumber)
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
            //SET THE PLAYER STATUS TO DEAD
            if (playerInstance != null)
            {
                playerInstance.p_Alive = false;
            }

        }
        if (col.gameObject.tag == "Terrain")
        {
            //isGrounded = true;
            waypoints[0].transform.position = transform.position - new Vector3(patrolDistance + transform.localScale.x, 0, 0);
            waypoints[1].transform.position = transform.position + new Vector3(patrolDistance + transform.localScale.x, 0, 0);
            Debug.Log("Touched the floor");
        }
    }
}
