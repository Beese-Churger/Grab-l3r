using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : EnemyBaseClass
{
    enum FSM
    {
        NEUTRAL,
        PATROL,
        NUM_STATE
    };

    //[SerializeField] private float x,y;
    [SerializeField] private GameObject[] waypoints;
    private FSM current;
    private int currentWP;
    private float speed;
    private float stationaryTimer;
    // Start is called before the first frame update
    void Start()
    {
        current = FSM.PATROL;
        //transform.position = new Vector2(x,y);
        currentWP = 0;
        speed = 5;
        stationaryTimer = 1;

        //Debug.Log("Size of WP array" + waypoints.Length);

    }
    void Update()
    {
        //FSMUpdate();
    }
    public override void FSMUpdate()
    {
        switch (current)
        {
            case FSM.NEUTRAL:
            Stop();
            break;
            case FSM.PATROL:
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
            current = FSM.PATROL;
            //Debug.Log("Timer over moving to wp" + currentWP);
        }
    }
}
