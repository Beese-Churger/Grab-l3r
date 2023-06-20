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
    FSM current = FSM.IDLE;
    ATTACK currentAttack;

    // Boss Level variables
    [SerializeField] GameObject bodyDrop;
    [SerializeField] GameObject horizontalPlatform;
    [SerializeField] GameObject verticalPlatform;
    private int phase = 0;
    private int prevPhaseNumber = 0;
    private int usableAbilities = 1;
    [SerializeField] private float chargeTimer = 3f;
    private float timer;
    private bool abilityUpdated = false;
    private GameObject playerGO;


    void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        timer = chargeTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // FSM UPDATE
        switch (current)
        {
            case FSM.IDLE:
                abilityUpdated = false;
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
                    Attack();
                }
                break;
            case FSM.DEAD:
                break;
        }
    }
    private void PhaseUpdate()
    {
        // TO DO: If pressure plate is triggered , the boss will move up to the next phase 
        phase++;
        usableAbilities++;
    }
    private void AbilityUpdate()
    {
        // TO DO: Update the number of usable abilities when the boss enters another phase
        
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
            ATTACK randomNo = (ATTACK)Random.Range(1, usableAbilities);
            currentAttack = randomNo;
        }
        else
        {
            currentAttack = ATTACK.SLAM;
        }
        abilityUpdated = true;

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
                }
                else if (dir.x < 0)
                {
                    // Use Left Hand to slam the player
                }
                break;
            case ATTACK.GRINDER:
                // Logic for the Grinder attack
                /* TO DO: Turn on the electric platforms 
                          (Not sure if the electric platforms
                           are already capable of moving, so just
                           in case that's an issue, make the electric platforms
                           translate between 2 points)*/
                break;
            case ATTACK.CRUSH:
                // Logic for the crush attack
                break;
        }
    }
    private void SpawnBody()
    {
        /* Spawns a body every time the boss enters a new phase
           Check if there's a change in the phase number*/
        if (phase != prevPhaseNumber)
        {
            prevPhaseNumber = phase;

            // Code to spawn the bodies
            for (int i = 0; i <= phase; i++)
            {
                float minX = transform.position.x - horizontalPlatform.transform.localScale.x + 5.0f;
                float minY = transform.position.y - verticalPlatform.transform.localScale.y;
                float maxX = transform.position.x + horizontalPlatform.transform.localScale.x;
                float maxY = transform.position.y + verticalPlatform.transform.localScale.y;
                Instantiate(bodyDrop);
            }

        }

    }
    private bool ChargeUp()
    {
        // TO DO: Spawn a beam of light above the player
        // 1 second before the timer runs out the beam will stop following the player
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return false;
        }
        else
            return true;
    }
}
