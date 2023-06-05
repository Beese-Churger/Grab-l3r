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
    private PlayerController playerInstance;

    // Start is called before the first frame update
    void Start()
    {
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        current = FSM.PATROL;
        currentWP = 0;
        speed = 25;
        stationaryTimer = 1;
        rotation = 180;
        transform.localRotation = Quaternion.Euler(0, rotation, 0);

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
                if (Vector2.Distance(transform.position, waypoints[0].transform.position) < 0.05 && currentWP == 0)
                {
                    currentWP = 1;
                    current = FSM.NEUTRAL;
                }
                if (Vector2.Distance(transform.position, waypoints[1].transform.position) < 0.05 && currentWP == 1)
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

    private void Patrol()
    {
        Vector3 dir = (waypoints[currentWP].transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        
        //transform.position = Vector2.Lerp(transform.position, waypoints[currentWP].transform.position, Time.deltaTime);
        //Debug.Log("ENEMY POSITION: " + gameObject.transform.position);
        //Debug.Log("WAYPOINT POSITION: " + waypoints[currentWP].transform.position);
        //Debug.Log("Big Enemy Going Towards WP "+ currentWP);
    }

    // Slows big enemy movement down as it approaches the waypoint,
    private void Slow()
    {
        if (Vector2.Distance(transform.position, waypoints[currentWP].transform.position) < 2)
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
        // Timer for the one second interval between waypoints
        if (stationaryTimer > 0)
        {
            stationaryTimer -= Time.deltaTime;
        }
        else
        {
            stationaryTimer = 1;
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
       //if (Vector2.Distance(transform.position, playerPrefab.transform.position) > 2)
        {
            Vector3 dir = (playerPrefab.transform.position - transform.position).normalized;
            dir.y = 0;
            transform.position += dir * speed * Time.deltaTime;
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

}
