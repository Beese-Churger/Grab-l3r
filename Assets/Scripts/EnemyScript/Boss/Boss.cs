using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Boss : MonoBehaviour
{
    public static Boss instance = null;
    // Start is called before the first frame update
    public enum ATTACK
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

    // Fixed spawnpoints for the body
    [SerializeField] private Transform bodySpawnpoint1;
    [SerializeField] private Transform bodySpawnpoint2;
    private List<object> spawnpointArray;

    [SerializeField] private GameObject horizontalPlatform;
    [SerializeField] private GameObject verticalPlatform;
    [SerializeField] private float gracePeriod = 3f;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask pPlateLayer;
    private float chargeTimer = 3f;


    // Body Spawning Variables
    private float m_Scale = 0.35f;
    private float raycastDistance = 20f;
    private float timer, graceTimer;
    private bool abilityUpdated = false;

    private GameObject playerGO;
    private GameObject bossHead;
    private GameObject bossBeam;
    private Color beam;
    private SpriteRenderer bossBeamColor;
    private Collider2D CrushAOE;
    private GameObject[] bossLeftArm;
    private GameObject[] bossRightArm;
    private List<object> attackArray;

    // Flag that controls the beam tracking the player
    private bool beenTracked = false;


    // Electric Platforms variable
    [SerializeField] ElectricPlatform controlledPlatform;
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
    private ATTACK prevAttack;

    private bool attacking = false;


    [Tooltip("Developer Mode.")]
    public bool invicibility = true;

    [Tooltip("Random Body Spawn Timer.")]
    [SerializeField] private float bodySpawnTimer;
    private float storeSpawnTimer;

    [Tooltip("First Possible Attack Interval.")]
    [SerializeField] private float firstPos;

    [Tooltip("Second Possible Attack Interval.")]
    [SerializeField] private float secondPos;

    private List<object> intervalArray;
    private bool safe;

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
        bossBeamColor = bossBeam.GetComponentInChildren<SpriteRenderer>();
        beam = bossBeamColor.color;
        electricPlatforms = GameObject.FindGameObjectsWithTag("ElectricPlatform");

        bossLeftArm = GameObject.FindGameObjectsWithTag("BossLeftArm");
        bossRightArm = GameObject.FindGameObjectsWithTag("BossRightArm");

        CrushAOE = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        timer = chargeTimer;
        graceTimer = gracePeriod;
        storeSpawnTimer = bodySpawnTimer;
        bossBeam.SetActive(false);

        intervalArray = new()
        {
            firstPos,
            secondPos
        };
        spawnpointArray = new()
        {
            bodySpawnpoint1,
            bodySpawnpoint2
        };
        attackArray = new()
        {
            ATTACK.SLAM
        };

 
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


                SafeZoneManager.instance.FlashZones(false);
                attacking = false;
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
                if (attacking)
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

        if (bodySpawnTimer > 0)
        {
            bodySpawnTimer -= Time.deltaTime;
        }
        else
        {
            SpawnBodyRandom();
            bodySpawnTimer = storeSpawnTimer;
        }
    }
    private void PhaseUpdate()
    {
        if (pTriggered)
        {
            //Debug.Log("Triggered");
            /*
             * Check if it reaches a new phase
             * Condition 2 checks whether or not the phase has been reached before
             * 
             */
            if (phase == prevPhaseNumber || phase < highestHitPhase)
            {
                // TO DO: If pressure plate is triggered , the boss will move up to the next phase 
                phase++;
                SpawnBody();
                Debug.Log("Phase Increase:" + phase);
            }
            pTriggered = false;


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
            while (prevAttack == currentAttack)
            {
                System.Random rand1 = new();
                Shuffle(rand1, attackArray);
                currentAttack = (ATTACK)attackArray[0];
            }
            Debug.Log(currentAttack);
        }
        else
        {
            currentAttack = ATTACK.SLAM;
        }
        abilityUpdated = true;
        prevAttack = currentAttack;

        //Debug.Log("Current Attack:" + currentAttack);

    }
    /* The logic of the different attacks */
    private void Attack()
    {
        safe = SafeZoneManager.instance.CheckSafeZone();
        switch (currentAttack)
        {
            case ATTACK.SLAM:
                SafeZoneManager.instance.FlashZones(true);
                // Logic for the slam attack
                Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;
                // Check if one side animation is playing already,
                // If the left arm animation is playing, only track left side even though the player
                // has left the right zone etc.)
                //////////////////////////////////////////////////////////////////////////////////////////////
                if (dir.x > 0)
                {
                    // Use Right Hand to slam the player
                    // Debug.Log("Attacking Right");
                    if (!isPlaying)
                    {
                        animator.SetBool("SlamR", true);
                        AudioManager.Instance.PlaySFX("boss_slam" + Random.Range(1, 2), transform.position);
                        left = false;
                        isPlaying = true;
                    }

                }
                else if (dir.x < 0)
                {
                    // Use Left Hand to slam the player             
                    //Debug.Log("Attacking Left");
                    if (!isPlaying)
                    {
                        animator.SetBool("SlamL", true);
                        AudioManager.Instance.PlaySFX("boss_slam" + Random.Range(1, 2), transform.position);
                        left = true;
                        isPlaying = true;
                    }

                }
                /////////////////////////////////////////////////////////////////////////////////////////////////
                if (left)
                {
                    Vector2 newDir = (playerGO.transform.position - iKLeftHand.GetChain(2).effector.position).normalized;
                    Vector3 newPos = newDir * 20f;
                    // If the player is initially in the safe zone, dont track anything because it will make the animation
                    // look weird
                    if (!slamming)
                    {
                        if (!safe)
                        {
                            iKLeftHand.GetChain(0).target = playerGO.transform;
                            beenTracked = true;
                        }
                        else
                        {
                            aimTarget.transform.position = defaultLeft.transform.position;
                            beenTracked = false;
                        }
                        slamming = true;
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (attackTimer < slamLClip.averageDuration * 0.6f)
                    {
                        Flash(true);

                        if (!safe)
                        {
                            aimTarget.transform.position = newPos + iKLeftHand.GetChain(2).effector.position;
                            beenTracked = true;
                        }
                        else
                            iKLeftHand.GetChain(0).target = aimTarget.transform;
                    }
                    else
                    {
                        iKLeftHand.GetChain(0).target = aimTarget.transform;
                        Flash(false);
                        SlamCollisionCheck(bossLeftArm);
                    }
                    if (attackTimer > slamLClip.averageDuration)
                    {
                        //Debug.Log("Slam L Animation done playing");
                        abilityUpdated = false;
                        isPlaying = false;
                        p_Hit = false;

                        attackTimer = 0f;
                        animator.SetBool("SlamL", false);
                        bossBeam.SetActive(false);
                        current = FSM.IDLE;
                        slamming = false;
                    }
                    else
                    {
                        attackTimer += Time.deltaTime;
                        //if (dir.x < 0)
                            ActivateBeam(ATTACK.SLAM);
                        //Debug.Log(attackTimer);
                    }

                }
                else
                {
                    Vector2 newDir = (playerGO.transform.position - iKRightHand.GetChain(2).effector.position).normalized;
                    Vector3 newPos = newDir * 20f;
                    if (!slamming)
                    {
                        if (!safe)
                        {
                            iKRightHand.GetChain(0).target = playerGO.transform;
                            beenTracked = true;
                        }
                        else
                        {
                            aimTarget.transform.position = defaultRight.transform.position;
                            beenTracked = false;

                        }

                        slamming = true;
                    }
                    if (attackTimer < slamRClip.averageDuration * 0.6f)
                    {
                        Flash(true);

                        if (!safe)
                            aimTarget.transform.position = newPos + iKRightHand.GetChain(2).effector.position;
                        else
                            iKRightHand.GetChain(0).target = aimTarget.transform;
                    }
                    else
                    {
                        iKRightHand.GetChain(0).target = aimTarget.transform;
                        Flash(false);
                        SlamCollisionCheck(bossRightArm);
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
                        bossBeam.SetActive(false);
                        current = FSM.IDLE;
                    }
                    else
                    {
                        attackTimer += Time.deltaTime;
                        //if (dir.x > 0)
                            ActivateBeam(ATTACK.SLAM);
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
                    AudioManager.Instance.PlaySFX("boss_grinder" + Random.Range(1, 2), transform.position);
                    isPlaying = true;
                }
                // During this specific frame of the attack activate the electric platforms
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
                    AudioManager.Instance.PlaySFX("boss_crush" + Random.Range(1, 2), transform.position);
                    isPlaying = true;
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
                    ActivateBeam(ATTACK.CRUSH);
                    //Debug.Log(crushTimer);
                }
                if (attackTimer > crushClip.averageDuration * 0.6f)
                    Flash(true);
                else
                    Flash(false);

                CrushCollisionCheck();
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
                for (int i = 0; i < phase; ++i)
                {
                    System.Random rand1 = new();
                    Shuffle(rand1, spawnpointArray);
                    Transform temp = (Transform)spawnpointArray[0];
                    Instantiate(bodyDrop, new Vector3(temp.position.x, temp.position.y, 0), Quaternion.identity, bodyDropHolder.transform);
                }
            }
            return;

        }
        else
        {
            prevPhaseNumber = phase;
        }

    }
    private void SpawnBodyRandom()
    {
        bool check = false;
        float minX = transform.position.x - horizontalPlatform.transform.localScale.x * m_Scale;
        float maxX = transform.position.x + horizontalPlatform.transform.localScale.x * m_Scale;
        float maxY = 0 + verticalPlatform.transform.localScale.x * m_Scale + 2.0f;
        // Code to spawn the bodies
        while (!check)
        {
            float x = Random.Range(minX, maxX);
            Vector3 leftRayOrigin = new Vector3(x, maxY, 0) + Vector3.left * raycastDistance;
            Vector3 rightRayOrigin = new Vector3(x, maxY, 0) + Vector3.right * raycastDistance;

            RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.left, 0f, platformLayer);
            RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, 0f, platformLayer);

            RaycastHit2D downHit = Physics2D.Raycast(new Vector3(x, maxY, 0), Vector2.down, 20f, pPlateLayer);

            if (leftHit.collider == null && rightHit.collider == null && !downHit)
            {
                Instantiate(bodyDrop, new Vector3(x, maxY, 0), Quaternion.identity, bodyDropHolder.transform);
                check = true;
                //Debug.Log("Spawned body");
            }
        }
            //Debug.Log(minX + "," + maxX);
            //Debug.Log(maxY);
    }

    private void ActivateBeam(ATTACK cur)
    {
        if (!bossBeam.activeInHierarchy)
            bossBeam.SetActive(true);

        if (cur == ATTACK.SLAM)
        {
            if (!safe && attackTimer < slamRClip.averageDuration * 0.6f)
            {
                Vector2 dir = (Vector2)playerGO.transform.position - (Vector2)bossHead.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(180f + angle, Vector3.forward);
                bossBeam.transform.localRotation = rotation;
                //Debug.Log(angle);
            }
            else if (!beenTracked)
            {
                Vector2 dir = (Vector2)aimTarget.transform.position - (Vector2)bossHead.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(180f + angle, Vector3.forward);
                bossBeam.transform.localRotation = rotation;
            }
            if (attackTimer > slamRClip.averageDuration)
                bossBeam.SetActive(false);
        }
        else if (cur == ATTACK.CRUSH)
        {
            Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            bossBeam.transform.localRotation = rotation;
            if (attackTimer > crushClip.averageDuration)
                bossBeam.SetActive(false);
        }


    }

    private void CrushCollisionCheck()
    { 
        if (currentAttack == ATTACK.CRUSH)
        {
            if (CrushAOE.OverlapPoint(playerGO.transform.position) && 
                attackTimer > crushClip.averageDuration * 0.6f &&
                !p_Hit && 
                !invicibility)
            {
                Debug.Log("Crush hit");
                GameManager.instance.TakeDamage();
                p_Hit = true;
                return;
            }
        }        
    }
    private void SlamCollisionCheck(GameObject[] side)
    {
        if (currentAttack == ATTACK.SLAM)
        {
            foreach (GameObject bossArmComponent in side)
            {
                if (bossArmComponent.GetComponent<Collider2D>().OverlapPoint(playerGO.transform.position) &&
                attackTimer > slamLClip.averageDuration * 0.8f &&
                !p_Hit && 
                !safe &&
                !invicibility)
                {
                    Debug.Log("Slam Hit!");
                    GameManager.instance.TakeDamage();
                    p_Hit = true;
                    return;
                }
            }
        }
    }
    private bool ChargeUp()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return false;
        }
        else
        {
            current = FSM.ATTACK;
            attacking = true;
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
            RandInterval();
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
    private void RandInterval()
    {
        System.Random rand1 = new();
        Shuffle(rand1, intervalArray);
        chargeTimer = (float)intervalArray[0];
        timer = chargeTimer;
        //Debug.Log(chargeTimer);
    }
    public void Shuffle(System.Random rng, List<object> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            object temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
    bool dir = true;
    private void Flash(bool isFlash)
    {
        if (isFlash)
        {
            if (bossBeamColor.color.a <= 0.5f && dir)
                beam.a += Time.deltaTime * 0.8f;
            else if (bossBeamColor.color.a >= 0.5f)
                dir = false;
            if (bossBeamColor.color.a >= 0.1f && !dir)
                beam.a -= Time.deltaTime * 0.8f;
            else if (bossBeamColor.color.a <= 0.1f)
                dir = true;
        }
        else
            beam.a = 0.5f;

        bossBeamColor.color = beam;
    }
    public void AddAttack(ATTACK newAttack)
    {
        if (!attackArray.Contains(newAttack))
        {
            //Debug.Log("Add attack");
            attackArray.Add(newAttack);
        }
    }
    public void RemoveAttack(ATTACK name)
    {
        if (attackArray.Contains(name))
        {
            //Debug.Log("Remove attack");
            attackArray.Remove(name);
        }
    }
    public void SetElectric(bool val)
    {
        controlledPlatform.activate = val;
    }
    public void SetPPlate(bool active)
    {
        pTriggered = active;
        // If pressure plate has been released the Phase Number decreases
        if (!active)
        {
            phase--;
            Debug.Log("Phase Decrease:" + phase);
        }
    }

}
