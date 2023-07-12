using UnityEngine;
using UnityEngine.U2D.IK;

public class Boss : MonoBehaviour
{
    public static Boss instance = null;
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
    [SerializeField] private GameObject bodyDrop;
    [SerializeField] private GameObject bodyDropHolder;

    [SerializeField] private GameObject horizontalPlatform;
    [SerializeField] private GameObject verticalPlatform;
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
    private Collider2D CrushAOE;
    private GameObject[] bossArm;

    // Electric Platforms variable
    private GameObject[] electricPlatforms;
    private bool electricActive;
    private float eTimer = 10f;
    private const float eVal = 10f;


    // Boss animation variables
    [SerializeField] private AnimationClip crushClip;
    [SerializeField] private AnimationClip grinderClip;
    [SerializeField] private AnimationClip slamLClip;
    [SerializeField] private AnimationClip slamRClip;
    [SerializeField] private LimbSolver2D iKLeftHand;
    [SerializeField] private LimbSolver2D iKRightHand;

    private GameObject defaultTarget;

    private Animator animator;
    private bool isPlaying = false;
    private float attackTimer = 0.0f;
    private bool p_Hit = false;
    private bool slamming = false;
    private bool left = true;
    private bool trackPlayer = false;
    // Boss Phase variable
    private bool pTriggered = false;
    private int phase = 0;
    private int highestHitPhase = 0;
    private int prevPhaseNumber = 0;
    private int usableAbilities = 0;
    private int pressedP = 0;
    private void Awake()
    {
        if (instance == null)
            instance = this;
   
    }
    private void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        defaultTarget = GameObject.Find("playerTracker");
        bossHead = GameObject.Find("Head");
        bossBeam = GameObject.Find("BossBeam");
        electricPlatforms = GameObject.FindGameObjectsWithTag("ElectricPlatform");

        bossArm = GameObject.FindGameObjectsWithTag("Boss");
        CrushAOE = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        timer = chargeTimer;
        graceTimer = gracePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates phase
        PhaseUpdate();
        // FSM UPDATE
        switch (current)
        {
            case FSM.IDLE:
                //Debug.Log("In Idle state");
                if (iKLeftHand.GetChain(0).target != null)
                {
                    iKLeftHand.GetChain(0).target = null;
                }

                if (iKRightHand.GetChain(0).target != null)
                    iKRightHand.GetChain(0).target = null;

                GracePeriod();
                break;
            case FSM.SCAN:
                // Spawn the beams 
                ChargeUp();
                if (!abilityUpdated)
                    GenerateSkill();
                break;
            case FSM.ATTACK:
                /* TO DO: Charge Up duration (3s)?
                 * First select which skill the boss is going to use
                   before the boss initiates the attack*/
                if (ChargeUp())
                    // Attack the player with an attack
                    Attack();
                break;
            case FSM.DEAD:
                break;
        }

        if (electricActive)
        {
            if (eTimer > 0f)
            {
                MoveEP();
                eTimer -= Time.deltaTime;
            }
            else
            {
                eTimer = eVal;
                electricActive = false;
            }
        }
    }
    private void PhaseUpdate()
    {
        if (pTriggered)
        {
            Debug.Log("Triggered");
            if (phase == prevPhaseNumber || phase < highestHitPhase)
            {
                // TO DO: If pressure plate is triggered , the boss will move up to the next phase 
                phase++;
                usableAbilities++;
                pressedP++;
                SpawnBody();
                Debug.Log("Phase:" + phase);
            }
            pTriggered = false;


        }
    }
    public void SetPPlate(bool active)
    {
        pTriggered = active;
        if (!active)
        {
            phase--;
            usableAbilities--;
            pressedP--;
            Debug.Log("Phase " + phase);
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
            System.Random rand1 = new();
            ATTACK randomNo = (ATTACK)rand1.Next(0, usableAbilities);
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
                Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;

                if (dir.x > 0)
                {
                    // Use Right Hand to slam the player
                    // TO DO: Play the right hand slam animation
                    // Only call on collision check at the frame when the boss is slaming
                    // Debug.Log("Attacking Right");
                    if (!isPlaying)
                    {
                        animator.SetBool("SlamR", true);
                        left = false;
                        isPlaying = true;
                    }

                }
                else if (dir.x < 0)
                {
                    // Use Left Hand to slam the player
                    // TO DO: Play the left hand slam animation                
                    //Debug.Log("Attacking Left");
                    if (!isPlaying)
                    {
                        animator.SetBool("SlamL", true);
                        left = true;
                        isPlaying = true;
                    }

                }
                if (left)
                {
                    Vector2 newDir = playerGO.transform.position - iKLeftHand.GetChain(2).effector.position;
                    Vector3 newPos = newDir * 5f;
                    if (!slamming)
                    {
                        iKLeftHand.GetChain(0).target = defaultTarget.transform;
                        slamming = true;
                    }
                    if (attackTimer < slamLClip.averageDuration * 0.6f)
                    {
                        defaultTarget.transform.position = newPos + iKLeftHand.GetChain(2).effector.position;
                        trackPlayer = true;
                    }
                    if (CollisionCheck())
                    {
                        p_Hit = true;
                    }
                    if (attackTimer > slamLClip.averageDuration)
                    {
                        Debug.Log("Slam L Animation done playing");
                        abilityUpdated = false;
                        isPlaying = false;
                        p_Hit = false;

                        attackTimer = 0f;
                        animator.SetBool("SlamL", false);
                        current = FSM.IDLE;
                        slamming = false;
                        trackPlayer = false;
                    }
                    else
                    {
                        attackTimer += Time.deltaTime;
                        //Debug.Log(attackTimer);
                    }
                }
                else
                {
                    Vector2 newDir = playerGO.transform.position - iKRightHand.GetChain(2).effector.position;
                    Vector3 newPos = newDir * 5f;
                    if (!slamming)
                    {
                        iKRightHand.GetChain(0).target = defaultTarget.transform;
                        slamming = true;
                    }
                    if (attackTimer < slamRClip.averageDuration * 0.6f)
                    {
                        defaultTarget.transform.position = newPos + iKRightHand.GetChain(2).effector.position;
                        trackPlayer = true;
                    }
                    if (CollisionCheck())
                    {
                        p_Hit = true;
                    }

                    // If slam right animation is done playing
                    /*
                     * Reset the timer that tracks whether the animation is done playing or not
                     * No animation is playing
                     * Update ability again
                     * */
                    if (attackTimer > slamRClip.averageDuration)
                    {
                        Debug.Log("Slam R Animation done playing");
                        abilityUpdated = false;
                        isPlaying = false;
                        p_Hit = false;
                        slamming = false;
                        trackPlayer = false;
                        attackTimer = 0f;
                        animator.SetBool("SlamR", false);
                        current = FSM.IDLE;
                    }
                    else
                    {
                        attackTimer += Time.deltaTime;
                        ActivateBeam();
                    }
                }


                break;
            case ATTACK.GRINDER:
                // Logic for the Grinder attack
                /* TO DO: Turn on the electric platforms 
                          (Not sure if the electric platforms
                           are already capable of moving, so just
                           in case that's an issue, make the electric platforms
                           translate between 2 points)*/
                if (!isPlaying)
                {
                    animator.SetBool("Electric", true);
                    isPlaying = true;
                }
                if (attackTimer > grinderClip.averageDuration * 0.7)
                {
                    electricActive = true;
                }
                if (attackTimer > grinderClip.averageDuration)
                {
                    Debug.Log("Grinder Animation done playing");
                    abilityUpdated = false;
                    isPlaying = false;
                    attackTimer = 0f;
                    animator.SetBool("Electric", false);
                    current = FSM.IDLE;

                }
                else
                    attackTimer += Time.deltaTime;

                break;
            case ATTACK.CRUSH:
                // Logic for the crush attack
                /* Play Crush Animation 
                 * Check whether the player is within the AOE of the attack
                 * If yes the player will lose a life
                 */
                // TO DO: Only do the check when it reaches a certain frame of the crush attack animation 
                if (!isPlaying)
                {
                    animator.SetBool("Crush", true);
                    isPlaying = true;
                }

                if (CollisionCheck())
                {
                    // Remove one life from the player
                    p_Hit = true;

                }
                if (attackTimer > crushClip.averageDuration)
                {
                    Debug.Log("Crush Animation done playing");
                    abilityUpdated = false;
                    isPlaying = false;
                    p_Hit = false;
                    attackTimer = 0f;
                    animator.SetBool("Crush", false);
                    current = FSM.IDLE;
                }
                else
                {
                    attackTimer += Time.deltaTime;
                    //Debug.Log(crushTimer);
                }

                break;
        }
    }
    private void SpawnBody()
    {
        /* Spawns a body every time the boss enters a new phase
           Check if there's a change in the phase number
        
         -------------DONE*/
        if (phase != prevPhaseNumber && phase > highestHitPhase)
        {
            prevPhaseNumber = phase;
            highestHitPhase = phase;

            if (phase <= 2)
            {
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
                            //Debug.Log("Spawned body");
                        }
                    }
                    check = false;
                    //Debug.Log(minX + "," + maxX);
                    //Debug.Log(maxY);
                }
            }
            return;

        }
        else
        {
            prevPhaseNumber = phase;
        }

    }
    private bool ChargeUp()
    {
        // TO DO: Spawn a beam of light above the player
        // Done
        //Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //Quaternion rotation = Quaternion.AngleAxis(180f + angle, Vector3.forward);
        //Debug.Log(angle);
        // 1 second before the timer runs out the beam will stop following the player
        //if (timer > 1f)
        //    bossBeam.transform.localRotation = rotation;
            //bossBeam.transform.rotation = Quaternion.Slerp(bossBeam.transform.rotation, rotation, 10f * Time.deltaTime);

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return false;
        }
        else
        {
            // Remove the beam
            //bossBeam.SetActive(false);
            current = FSM.ATTACK;
            return true;
        }
    }
    private void ActivateBeam()
    {
        if (!bossBeam.activeInHierarchy)
            bossBeam.SetActive(true);

        Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(180f + angle, Vector3.forward);
        //Debug.Log(angle);
        // 1 second before the timer runs out the beam will stop following the player
        if (attackTimer < slamRClip.averageDuration * 0.6f)
            bossBeam.transform.localRotation = rotation;
        //bossBeam.transform.rotation = Quaternion.Slerp(bossBeam.transform.rotation, rotation, 10f * Time.deltaTime);
        else
        {
            // Remove the beam
            bossBeam.SetActive(false);
        }
    }

    private bool CollisionCheck()
    {
        foreach (GameObject bossArmComponent in bossArm)
        {
            if (currentAttack == ATTACK.CRUSH)
            {
                if ((bossArmComponent.GetComponent<Collider2D>().OverlapPoint(playerGO.transform.position) || 
                    CrushAOE.OverlapPoint(playerGO.transform.position)) && 
                    attackTimer > crushClip.averageDuration * 0.6f &&
                    !p_Hit)
                {
                    Debug.Log("Crush hit");
                    //Debug.Log(bossArmComponent.name + " has collided with the player");
                    GameManager.instance.RemoveLife();
                    return true;
                }
            }
            else if (currentAttack == ATTACK.SLAM)
            {
                if (bossArmComponent.GetComponent<Collider2D>().OverlapPoint(playerGO.transform.position) &&
                    attackTimer > slamLClip.averageDuration * 0.8f &&
                    !p_Hit)
                {
                    Debug.Log("Slam Hit!");
                    GameManager.instance.RemoveLife();
                    return true;
                }
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
            graceTimer = gracePeriod;
            timer = chargeTimer;
            current = FSM.SCAN;

        }

    }
    private void MoveEP()
    {
        foreach (GameObject electricPlatform in electricPlatforms)
        {
            electricPlatform.GetComponent<ElectricPlatform>().SetActive();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (bossHead != null && playerGO != null)
            Gizmos.DrawLine(bossHead.transform.position, playerGO.transform.position);

    }


}
