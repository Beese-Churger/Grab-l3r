using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SimpleController : MonoBehaviour
{
    public static SimpleController Instance;
    public ParticleSystem dust;

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

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        playerPos = transform.position;
        checkpointPos = transform.position;
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
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
        horizontalInput = movement.action.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        float Accel = groundCheck ? AirAccel : GroundAccel;

        if (groundCheck)
        {
            if (airBorn)
            {
                if (!dust.isPlaying)
                {
                    var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
                    var newPos = new Vector3(transform.position.x, transform.position.y - halfHeight - 0.04f, transform.position.z);
                    dust.transform.position = newPos;
                    dust.Play();
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
                //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, Accel), 0), ForceMode2D.Force);
                if(isHooked)
                {
                    rBody.AddForce(new Vector2(Accel * 2, 0), ForceMode2D.Force);
                    //rBody.AddForce(new Vector2(0, -1), ForceMode2D.Force);
                }

                else
                    rBody.AddForce(new Vector2(Accel, 0), ForceMode2D.Force);
            }

            else if (horizontalInput < 0f)
            {
                //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0), ForceMode2D.Force);
                if (isHooked)
                {
                    rBody.AddForce(new Vector2(-Accel * 2, 0), ForceMode2D.Force);
                    //rBody.AddForce(new Vector2(0 , -1), ForceMode2D.Force);
                }

                else
                    rBody.AddForce(new Vector2(-Accel, 0), ForceMode2D.Force);
            }
            //else
            //{
            //    rBody.velocity = new Vector2(rBody.velocity.x * 0.99f, rBody.velocity.y);
            //}
        }

        // clamp the velocity to something sane
        if (rBody.velocity.magnitude > 100)
            rBody.velocity = rBody.velocity.normalized * 100;

        if(groundCheck)
            rBody.velocity = new Vector2(rBody.velocity.x * 0.9f, rBody.velocity.y);
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

    private float SaturatedAdd(float Min, float Max, float Current, float Modifier)
    {
        if (Modifier < 0)
        {
            if (Current < Min)
                return Current;
            Current += Modifier;
            if (Current < Min)
                Current = Min;
            return Current;
        }
        else
        {
            if (Current > Max)
                return Current;
            Current += Modifier;
            if (Current > Max)
                Current = Max;
            return Current;
        }
    }
}
