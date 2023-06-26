using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boss : MonoBehaviour
{
    // Start is called before the first frame update
    enum ATTACK
    {
        SLAM,
        GRINDER,
        CRUSH,
        NUM_ATTACKS
    };
    enum FSM
    {
        IDLE,
        SCAN,
        ATTACK,
        DEAD,
        NUM_STATE
    };

    // FSM variables
    FSM current = FSM.ATTACK;
    ATTACK currentAttack;

    // Boss Level variables
    [SerializeField] GameObject bodyDrop;
    [SerializeField] GameObject bodyDropHolder;

    [SerializeField] GameObject horizontalPlatform;
    [SerializeField] GameObject verticalPlatform;
    private int phase = 0;
    private int prevPhaseNumber = 0;
    private int usableAbilities = 1;
    [SerializeField] private float chargeTimer = 3f;
    [SerializeField] private float gracePeriod = 3f;
    [SerializeField] private LayerMask platformLayer;
    private float m_Scale = 0.35f;
    private float raycastDistance = 20f;
    private float timer, graceTimer;
    private bool abilityUpdated = false;

    private GameObject playerGO;
    private GameObject bossHead;
    private GameObject bossBeam;

    private GameObject[] bossArm;

    // temporary variable
    bool pTriggered = false;

    void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        bossHead = GameObject.Find("Head");
        bossBeam = GameObject.Find("BossBeam");

        bossArm = GameObject.FindGameObjectsWithTag("Boss");

        timer = chargeTimer;
        graceTimer = gracePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        // Pressure Plate triggered
        if (pTriggered)
        {
            PhaseUpdate();
            pTriggered = false;
        }
        // FSM UPDATE
        switch (current)
        {
            case FSM.IDLE:
                //Debug.Log("In Idle state");
                GracePeriod();
                break;
            case FSM.SCAN:
                // Spawn the beams 
                break;
            case FSM.ATTACK:
                /* TO DO: Charge Up duration (3s)?
                 * First select which skill the boss is going to use
                   before the boss initiates the attack*/
                if (!abilityUpdated)
                    GenerateSkill();

                if (ChargeUp())
                {
                    // Attack the player with an attack
                    Debug.Log("Boss is performing an attack");
                    Attack();
                }
                break;
            case FSM.DEAD:
                break;
        }
    }
    private void PhaseUpdate()
    {
        if (phase == prevPhaseNumber)
        {
            // TO DO: If pressure plate is triggered , the boss will move up to the next phase 
            phase++;
            usableAbilities++;
            SpawnBody();
        }
    }
    private void GenerateSkill()
    {
        /* TO DO: Check for which phase the boss is in,
         *        Determine which skills can be used in that phase,
         *        If there is more than one skill, generate a random number
         *        to determine which skill to use before the attack(during the charge up)
         */
        if (phase != 0)
        {
            // Generates a random number from 1 to the number of abilities the boss unlocked
            ATTACK randomNo = (ATTACK)Random.Range(1, usableAbilities);
            currentAttack = randomNo;
        }
        else
        {
            currentAttack = ATTACK.SLAM;
        }
        abilityUpdated = true;
        bossBeam.SetActive(true);

    }
    /* The logic of the different attacks */
    private void Attack()
    {
        switch (currentAttack)
        {
            case ATTACK.SLAM:
                // Logic for the slam attack
                Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)transform.position;
                if (dir.x > 0)
                {
                    // Use Right Hand to slam the player
                    // TO DO: Play the right hand slam animation
                    // Only call on collision check at the frame when the boss is slaming
                    Debug.Log("Attacking Right");
                    CollisionCheck();

                }
                else if (dir.x < 0)
                {
                    // Use Left Hand to slam the player
                    // TO DO: Play the left hand slam animation
                    Debug.Log("Attacking Left");
                    CollisionCheck();
                }

                current = FSM.IDLE;
                abilityUpdated = false;

                break;
            case ATTACK.GRINDER:
                // Logic for the Grinder attack
                /* TO DO: Turn on the electric platforms 
                          (Not sure if the electric platforms
                           are already capable of moving, so just
                           in case that's an issue, make the electric platforms
                           translate between 2 points)*/
                abilityUpdated = false;
                break;
            case ATTACK.CRUSH:
                // Logic for the crush attack
                /* Play Crush Animation 
                 * Check whether the player is within the AOE of the attack
                 * If yes the player will lose a life
                 */
                // TO DO: Only do the check when it reaches a certain frame of the crush attack animation 
                if (CollisionCheck())
                {
                    // Remove one life from the player

                }
                abilityUpdated = false;
                break;
        }
    }
    private void SpawnBody()
    {
        /* Spawns a body every time the boss enters a new phase
           Check if there's a change in the phase number
        
         -------------DONE*/
        if (phase != prevPhaseNumber)
        {
            prevPhaseNumber = phase;

            bool check = false;
            float minX = transform.position.x - horizontalPlatform.transform.localScale.x * m_Scale;
            float maxX = transform.position.x + horizontalPlatform.transform.localScale.x * m_Scale;
            float maxY = 0 + verticalPlatform.transform.localScale.x * m_Scale + 2.0f;
            // Code to spawn the bodies
            for (int i = 0; i < phase; ++i)
            {
                while (!check)
                {
                    float x = Random.Range(minX, maxX);
                    Vector3 leftRayOrigin = new Vector3(x, maxY, 0) + Vector3.left * raycastDistance;
                    Vector3 rightRayOrigin = new Vector3(x, maxY, 0) + Vector3.right * raycastDistance;

                    RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, 0f, platformLayer);
                    RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, 0f, platformLayer);
                    if (leftHit.collider == null || rightHit.collider == null)
                    {
                        Instantiate(bodyDrop, new Vector3(x, maxY, 0), Quaternion.identity, bodyDropHolder.transform);
                        check = true;
                    }
                }
                check = false;
                //Debug.Log(minX + "," + maxX);
                //Debug.Log(maxY);
            }

        }

    }
    private bool ChargeUp()
    {
        // TO DO: Spawn a beam of light above the player
        // Done
        Vector2 dir = (Vector2)bossHead.transform.position - (Vector2)playerGO.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Add 90 due to the fact that the beam is alr initially rotated by 90 degrees
        // 1 second before the timer runs out the beam will stop following the player
        if (timer > 1f)
            bossBeam.transform.rotation = Quaternion.AngleAxis(90 + angle, Vector3.forward);
        //Debug.Log(angle);
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return false;
        }
        else
        {
            // Remove the beam
            bossBeam.SetActive(false);
            timer = chargeTimer;
            return true;
        }
    }
    private bool CollisionCheck()
    {
        foreach (GameObject bossArmComponent in bossArm)
        {
            if (bossArmComponent.GetComponent<Collider2D>().OverlapPoint(playerGO.transform.position))
            {
                Debug.Log(bossArmComponent.name + " has collided with the player");
                return true;
            }
        }
        return false;
    }
    private void GracePeriod()
    {
        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Scan");
            current = FSM.ATTACK;
            graceTimer = gracePeriod;
        }
            
    }

}
