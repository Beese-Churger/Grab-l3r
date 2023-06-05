using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
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
    private SpriteRenderer playerSprite;
    private Rigidbody2D rBody;
    private bool isJumping;
    private Animator animator;
    private float jumpInput;
    private float horizontalInput;

    void Awake()
    {
        instance = this;
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
        if (horizontalInput < 0f || horizontalInput > 0f)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            playerSprite.flipX = horizontalInput < 0f;
            if (isSwinging)
            {
                animator.SetBool("IsSwinging", true);

                // Get normalized direction vector from player to the hook point
                var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

                // Inverse the direction to get a perpendicular direction
                Vector2 perpendicularDirection;
                if (horizontalInput < 0)
                {
                    perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                    var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                    Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                }
                else
                {
                    perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                    var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                    Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                }

                var force = perpendicularDirection * swingForce;
                rBody.AddForce(force, ForceMode2D.Force);
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

    //private void GrappleUpdate()
    //{
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mousePos.z = 0;

    //    Vector3 Dir = (Player.transform.position - mousePos).normalized;
    //    float angle = angle360(Dir, Vector3.Angle(Dir, Player.transform.up));

    //    //Debug.Log(angle);
    //    //GrappleGun.Instance.ShootGrapple(0f);
    //    //if (IsRight(angle))
    //    //{
    //    //    if (Input.GetKey(KeyCode.A))
    //    //    {
    //    //        Debug.Log("1");
    //    //    }
    //    //}
    //    //else 
    //    //{
    //    //    if (Input.GetKey(KeyCode.S))
    //    //    {
    //    //        Debug.Log("2");
    //    //    }
    //    //}


    //}

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
