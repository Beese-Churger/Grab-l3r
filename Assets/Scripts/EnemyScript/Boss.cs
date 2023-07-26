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
    FSM current = FSM.IDLE;
    ATTACK currentAttack;

    // Boss Level variables
    [SerializeField] private GameObject bodyDrop;
    [SerializeField] private GameObject bodyDropHolder;

    [SerializeField] private GameObject horizontalPlatform;
    [SerializeField] private GameObject verticalPlatform;
    [SerializeField] private float chargeTimer = 3f;
    [SerializeField] private float gracePeriod = 3f;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask pPlateLayer;

    // Body Spawning Variables
    private float m_Scale = 0.35f;
    private float raycastDistance = 20f;
    private float timer, graceTimer;
    private bool abilityUpdated = false;

    private GameObject playerGO;
    private GameObject bossHead;
    private GameObject bossBeam;
    private Collider2D CrushAOE;
    private GameObject[] bossLeftArm;
    private GameObject[] bossRightArm;
    private SafeZone safeZone;


    // Electric Platforms variable
    private GameObject[] electricPlatforms;
    private bool electricActive; // Activates the electric platforms
    private bool elecActivated = false; // Check whether pressure plate had activated/deactivated the electricity the grinder attack occurs / not

    private float eTimer = 10f;
    private const float eVal = 10f;


    // Boss animation variables
    [SerializeField] private AnimationClip crushClip;
    [SerializeField] private AnimationClip grinderClip;
    [SerializeField] private AnimationClip slamLClip;
    [SerializeField] private AnimationClip slamRClip;
    [SerializeField] private LimbSolver2D iKLeftHand;
    [SerializeField] private LimbSolver2D iKRightHand;

    private GameObject aimTarget;
    private GameObject defaultLeft;
    private GameObject defaultRight;

    private Animator animator;
    private bool isPlaying = false;
    private float attackTimer = 0.0f;
    private bool p_Hit = false;
    private bool slamming = false;
    private bool left = true;
    // Boss Phase variable
    private bool pTriggered = false;
    private int phase = 0;
    private int highestHitPhase = 0;
    private int prevPhaseNumber = 0;
    private int usableAbilities = 0;
    private int pressedP = 0;


    [Tooltip("Developer Mode.")]
    public bool invicibility = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
   
    }
    private void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        aimTarget = GameObject.Find("playerTracker");
        defaultLeft = GameObject.Find("leftSideMarker");
        defaultRight = GameObject.Find("rightSideMarker");
        bossHead = GameObject.Find("Head");
        bossBeam = GameObject.Find("BossBeam");
        electricPlatforms = GameObject.FindGameObjectsWithTag("ElectricPlatform");

        bossLeftArm = GameObject.FindGameObjectsWithTag("BossLeftArm");
        bossRightArm = GameObject.FindGameObjectsWithTag("BossRightArm");

        safeZone = GameObject.Find("SafeZone").GetComponent<SafeZone>();

        CrushAOE = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        timer = chargeTimer;
        graceTimer = gracePeriod;
        bossBeam.SetActive(false);
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

        if (elecActivated)
        {
            if (eTimer > 0f)
            {
                eTimer -= Time.deltaTime;
            }
            else
            {
                eTimer = eVal;
                electricActive = false;
                elecActivated = false;
            }
        }
        MoveEP(electricActive);

    }
    public void SetElectric(bool val)
    {
        if (currentAttack == ATTACK.GRINDER && elecActivated)
            electricActive = val;
    }
    private void PhaseUpdate()
    {
        if (pTriggered)
        {
            Debug.Log("Triggered");
            /*
             * Check if it reaches a new phase
             * Condition 2 checks whether or not the phase has been reached before
             * 
             */
            if (phase == prevPhaseNumber || phase < highestHitPhase)
            {
                // TO DO: If pressure plate is triggered , the boss will move up to the next phase 
                phase++;
                usableAbilities++;
                pressedP++;
                SpawnBody();
                Debug.Log("Phase Increase:" + phase);
            }
            pTriggered = false;


        }
    }
    public void SetPPlate(bool active)
    {
        pTriggered = active;
        //Debug.Log("Phase " + phase);
        // If pressure plate has been released the Phase Number decreases
        if (!active)
        {
            phase--;
            usableAbilities--;
            pressedP--;
            Debug.Log("Phase Decrease" + phase);
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
        //bossBeam.SetActive(true);
        //Debug.Log("Current Attack:" + currentAttack);

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
                    Vector2 newDir = (playerGO.transform.position - iKLeftHand.GetChain(2).effector.position).normalized;
                    Vector3 newPos = newDir * 20f;
                    if (!slamming)
                    {
                        if (!safeZone.playerSafe)
                            iKLeftHand.GetChain(0).target = playerGO.transform;
                        else
                            iKLeftHand.GetChain(0).target = defaultLeft.transform;
                        slamming = true;
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (attackTimer < slamLClip.averageDuration * 0.6f)
                    {
                        if (!safeZone.playerSafe)
                            aimTarget.transform.position = newPos + iKLeftHand.GetChain(2).effector.position;
                        else
                            iKLeftHand.GetChain(0).target = aimTarget.transform;
                    }
                    else
                    {
                        iKLeftHand.GetChain(0).target = aimTarget.transform;
                    }
                    if (SlamCollisionCheck(bossLeftArm))
                    {
                        Debug.Log("Left Hand Hit");
                        p_Hit = true;
                    }
                    if (attackTimer > slamLClip.averageDuration)
                    {
                        //Debug.Log("Slam L Animation done playing");
                        abilityUpdated = false;
                        isPlaying = false;
                        p_Hit = false;

                        attackTimer = 0f;
                        animator.SetBool("SlamL", false);
                        current = FSM.IDLE;
                        slamming = false;
                    }
                    else
                    {
                        attackTimer += Time.deltaTime;
                        ActivateBeam();
                        //Debug.Log(attackTimer);
                    }
                }
                else
                {
                    Vector2 newDir = (playerGO.transform.position - iKRightHand.GetChain(2).effector.position).normalized;
                    Vector3 newPos = newDir * 20f;
                    if (!slamming)
                    {
                        if (!safeZone.playerSafe)
                            iKRightHand.GetChain(0).target = playerGO.transform;
                        else
                            iKRightHand.GetChain(0).target = defaultRight.transform;
                        slamming = true;
                    }
                    if (attackTimer < slamRClip.averageDuration * 0.6f)
                    {
                        if (!safeZone.playerSafe)
                            aimTarget.transform.position = newPos + iKRightHand.GetChain(2).effector.position;
                        else
                            iKRightHand.GetChain(0).target = aimTarget.transform;
                    }
                    else
                    {
                        iKRightHand.GetChain(0).target = aimTarget.transform;
                    }
                    if (SlamCollisionCheck(bossRightArm))
                    {
                        Debug.Log("Right Hand Hit");
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
                        //Debug.Log("Slam R Animation done playing");
                        abilityUpdated = false;
                        isPlaying = false;
                        p_Hit = false;
                        slamming = false;
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
                    elecActivated = true;
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

                        RaycastHit2D downHit = Physics2D.Raycast(new Vector3(x, maxY, 0), Vector2.down, 3f, pPlateLayer);

                        if (leftHit.collider == null && rightHit.collider == null && !downHit)
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
    private void ActivateBeam()
    {
        if (!bossBeam.activeInHierarchy)
            bossBeam.SetActive(true);

        if (!safeZone.playerSafe)
        {
            Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(180f + angle, Vector3.forward);

            //Debug.Log(angle);
            bossBeam.transform.localRotation = rotation;
        }
        if (attackTimer > slamRClip.averageDuration)
            bossBeam.SetActive(false);


    }

    private bool CollisionCheck()
    { 
        if (currentAttack == ATTACK.CRUSH)
        {
            if (CrushAOE.OverlapPoint(playerGO.transform.position) && 
                attackTimer > crushClip.averageDuration * 0.6f &&
                !p_Hit && 
                !invicibility)
            {
                Debug.Log("Crush hit");
                //Debug.Log(bossArmComponent.name + " has collided with the player");
                GameManager.instance.TakeDamage();
                return true;
            }
        }        
        return false;
    }
    private bool SlamCollisionCheck(GameObject[] side)
    {
        if (currentAttack == ATTACK.SLAM)
        {
            foreach (GameObject bossArmComponent in side)
            {
                if (bossArmComponent.GetComponent<Collider2D>().OverlapPoint(playerGO.transform.position) &&
                attackTimer > slamLClip.averageDuration * 0.8f &&
                !p_Hit && 
                !safeZone.playerSafe &&
                !invicibility)
                {
                    Debug.Log("Slam Hit!");
                    GameManager.instance.TakeDamage();
                    return true;
                }
            }
        }
        return false;
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
    private void GracePeriod()
    {
        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }
        else
        {
            //Debug.Log("Scan");
            graceTimer = gracePeriod;
            timer = chargeTimer;
            current = FSM.SCAN;

        }

    }
    private void MoveEP(bool move)
    {
        foreach (GameObject electricPlatform in electricPlatforms)
        {
            electricPlatform.GetComponent<ElectricPlatform>().SetActive(move);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (bossHead != null && playerGO != null)
            Gizmos.DrawLine(bossHead.transform.position, playerGO.transform.position);

    }


}
