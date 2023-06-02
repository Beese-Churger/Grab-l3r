using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemy : EnemyBaseClass
{
    enum FSM
    {
        NEUTRAL,
        PATROL,
        AGGRESSIVE,
        NUM_STATE
    };
    //[SerializeField] private float x,y;
    [SerializeField] private GameObject[] waypoints;
    private GameObject playerPrefab;
    private FSM current;
    private int currentWP;
    private float speed;
    private float stationaryTimer;
    private float rotation;
    private PolygonCollider2D FOV;

    // Start is called before the first frame update
    void Start()
    {
        FOV = transform.Find("FOV").gameObject.GetComponent<PolygonCollider2D>();
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        current = FSM.PATROL;
        currentWP = 0;
        speed = 5;
        stationaryTimer = 1;
        rotation = 180;
        transform.localRotation = Quaternion.Euler(0, rotation, 0);

    }
    void Update()
    {
        //FSMUpdate();
    }
    public override void FSMUpdate()
    {
        switch (current)
        {
            case FSM.NEUTRAL: // For NEUTRAL State, The enemy temporarily stops moving before it starts moving again
            Stop();
            break;
            case FSM.PATROL:   
            // For PATROL State, The enemy would be patrolling around it's own platform to find the player
            if (Vector2.Distance(transform.position, waypoints[0].transform.position) < 0.05)
            {
                currentWP = 1;
                current = FSM.NEUTRAL;
            }
            if (Vector2.Distance(transform.position, waypoints[1].transform.position) < 0.05)
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
            break;

        }
    }
    private void Patrol()
    {
        Vector3 dir = (waypoints[currentWP].transform.position - gameObject.transform.position).normalized;
        gameObject.transform.position += dir * speed * Time.deltaTime;
        
        //transform.position = Vector2.Lerp(transform.position, waypoints[currentWP].transform.position, Time.deltaTime);
        //Debug.Log("ENEMY POSITION: " + gameObject.transform.position);
        //Debug.Log("WAYPOINT POSITION: " + waypoints[currentWP].transform.position);
        //Debug.Log("Big Enemy Going Towards WP "+ currentWP);
    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        if (Vector2.Distance(gameObject.transform.position, waypoints[currentWP].transform.position) < 2)
        {
            if (speed > 0)
            {
                speed -= 6 * Time.deltaTime;
            }
        }
        else
        {
            speed = 5;
        }           
    }
    /* Make the big enemy stationary for 1 second before allowing it to move to
       the next waypoint
     */
    private void Stop()
    {
        if (stationaryTimer > 0)
        {
            stationaryTimer -= Time.deltaTime;
        }
        else
        {
            stationaryTimer = 1;
            rotation -= 180;
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
            current = FSM.PATROL;
            //Debug.Log("Timer over moving to wp" + currentWP);
        }
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

}
