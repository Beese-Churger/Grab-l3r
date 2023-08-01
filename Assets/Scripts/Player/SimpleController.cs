using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SimpleController : MonoBehaviour
{
    public static SimpleController Instance;

    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference jump;
    private Rigidbody2D rBody;
    public float MAXSPEED = 100f;
    public float AirAccel = 3f;
    public float GroundAccel = 0f;
    private float jumpInput;
    private float horizontalInput;
    public bool groundCheck;
    public float jumpSpeed = 3f;
    public bool isJumping = false;
    private bool isHooked = false;
    private Vector2 playerPos;
    private Vector2 checkpointPos;
    private bool airBorn = false;

    [SerializeField] private GameObject DustPrefab;
    public LayerMask ropeLayerMask;
    float halfHeight;

    // idle sounds
    private float lastBeepTime;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        playerPos = transform.position;
        checkpointPos = transform.position;
        halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        lastBeepTime = Time.deltaTime;
    }

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        if (GameManager.instance.resetPlayer == true)
        {
            transform.position = GameManager.instance.checkpointPos;
            GameManager.instance.resetPlayer = false;
        }
        else
        {
            transform.position = playerPos;
        }
    }

    public Vector2 GetCheckpoint()
    {
        return checkpointPos;
    }

    // Update is called once per frame
    void Update()
    {
        jumpInput = jump.action.ReadValue<float>();
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f, ropeLayerMask);
        horizontalInput = movement.action.ReadValue<Vector2>().x;

        float Accel = groundCheck ? AirAccel : GroundAccel;

        if (groundCheck)
        {
            if (airBorn)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f, ropeLayerMask);
                GameObject dust = Instantiate(DustPrefab,hit.point,Quaternion.identity);
                Destroy(dust, dust.GetComponent<ParticleSystem>().main.duration);

                // if land on metal else...
                if(hit)
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Pivot"))
                    {
                        AudioManager.Instance.PlaySFX("player_land_metal" + Random.Range(1, 3), transform.position);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX("player_land_sand" + Random.Range(1, 5), transform.position);
                    }
                }
              
                airBorn = false;
            }

            isJumping = jumpInput > 0f;
            if (isJumping)
            {
                rBody.velocity = new Vector2(rBody.velocity.x, jumpSpeed);
            }
        }
        else
        {
            airBorn = true;
            if (horizontalInput > 0f)
            {
                if(isHooked)
                {
                    rBody.AddForce(new Vector2(Accel * 2, 0), ForceMode2D.Force);
                }
                else
                    rBody.AddForce(new Vector2(Accel, 0), ForceMode2D.Force);
            }

            else if (horizontalInput < 0f)
            {
                if (isHooked)
                {
                    rBody.AddForce(new Vector2(-Accel * 2, 0), ForceMode2D.Force);
                }
                else
                    rBody.AddForce(new Vector2(-Accel, 0), ForceMode2D.Force);
            }
        }

        // clamp the velocity to something sane
        if (rBody.velocity.magnitude > 100)
            rBody.velocity = rBody.velocity.normalized * 100;

        if(groundCheck)
            rBody.velocity = new Vector2(rBody.velocity.x * 0.9f, rBody.velocity.y);

        if(lastBeepTime + Random.Range(20,30)< Time.time && groundCheck)
        {
            AudioManager.Instance.PlaySFX("player_idle", transform.position);
            lastBeepTime = Time.time;
        }
    }

    public void damageTaken()
    {
        if (gameObject.GetComponent<throwhook>().ropeActive)
            gameObject.GetComponent<throwhook>().destroyHook();
        rBody.velocity = -rBody.velocity.normalized * 5;

        AudioManager.Instance.PlaySFX("player_damaged" + Random.Range(1, 5), transform.position);
    }

    public void SetCheckPoint(Vector2 point)
    {
        checkpointPos = point;
    }

    public void SetHook(bool _hook)
    {
        isHooked = _hook;
    }
}
