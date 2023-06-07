using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    //public bool isSwinging = false;
    //public Vector2 ropeHook;
    //public float swingForce = 4f;

    //[SerializeField] private GameObject Grapple1;
    //[SerializeField] private GameObject Grapple2;
    //[SerializeField] private GameObject Player;

    // A variable that checks whether the player is alive or not
    public bool p_Alive = true;

    public float swingForce = 4f;
    public float speed = 1f;
    public float jumpSpeed = 3f;
    public Vector2 ropeHook;
    public bool isSwinging;
    public bool groundCheck;

    public float MAXSPEED = 1000f;
    public float AirAccel = 100.0f;
    public float GroundAccel = 100.0f;
    private SpriteRenderer playerSprite;
    private Rigidbody2D rBody;
    private bool isJumping;
    private Animator animator;
    private float jumpInput;
    private float horizontalInput;

    enum HookState
    {
        HOOK_RETRACTED,
        HOOK_IDLE,
        HOOK_RETRACT_START,
        HOOK_RETRACT_END,
        HOOK_FLYING,
        HOOK_GRABBED,
        HOOK_GRABBED_NOHOOK,
        HOOK_ATTACH_GROUND,
        HOOK_ATTACH_ENTITY,
    };
   
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerSprite = GetComponent<SpriteRenderer>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        jumpInput = Input.GetAxis("Jump");
        horizontalInput = Input.GetAxis("Horizontal");
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
    }

    void FixedUpdate()
    {
        // determine how much the player is to accelerate
        float Accel = isSwinging ? AirAccel : GroundAccel;
        if (horizontalInput < 0f || horizontalInput > 0f)
        {
            //animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            playerSprite.flipX = horizontalInput < 0f;
            if (isSwinging)
            {
                //// animator.SetBool("IsSwinging", true);

                // // Get normalized direction vector from player to the hook point
                // var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

                // // Inverse the direction to get a perpendicular direction
                // Vector2 perpendicularDirection;
                // if (horizontalInput < 0)
                // {
                //     perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                //     var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                //     Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                // }
                // else
                // {
                //     perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                //     var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                //     Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                // }

                // var force = perpendicularDirection * swingForce;
                // rBody.AddForce(force, ForceMode2D.Force);
                //rBody.velocity = new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0);
                rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0), ForceMode2D.Force);
            }
            else
            {
                animator.SetBool("IsSwinging", false);
                if (groundCheck)
                {
                    var groundForce = speed * 2f;
                    rBody.AddForce(new Vector2((horizontalInput * groundForce - rBody.velocity.x) * groundForce, 0));
                    rBody.velocity = new Vector2(rBody.velocity.x, rBody.velocity.y);
                }
            }
        }
        else
        {
            animator.SetBool("IsSwinging", false);
            animator.SetFloat("Speed", 0f);
        }

        if (!isSwinging)
        {
            if (!groundCheck) return;

            isJumping = jumpInput > 0f;
            if (isJumping)
            {
                rBody.velocity = new Vector2(rBody.velocity.x, jumpSpeed);
            }
        }
    }

    // increases or decreases speed at first, but only up to a maximum (saturation) level
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

    //private bool IsRight(float angle)
    //{
    //    if (angle > 180)
    //    {
    //        return false;
    //    }
    //    return true;
    //}
    //float angle360(Vector3 dir, float theangle) // calculate 360 angle using Vec3.Angle
    //{
    //    float angle2 = Vector3.Angle(dir, this.gameObject.transform.right);
    //    if (angle2 > 90f)
    //    {
    //        theangle = 360f - theangle;
    //    }
    //    return theangle;
    //}
}
