using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SimpleController : MonoBehaviour
{
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
    private RopeScript ropeScript;
    private bool isHooked = false;
    // Start is called before the first frame update
    void Start()
    {
  
        rBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        jumpInput = jump.action.ReadValue<float>();
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
        horizontalInput = movement.action.ReadValue<Vector2>().x;
        //horizontalInput = Input.GetAxis("Horizontal");

        //Debug.Log("H_In:" + horizontalInput);

    }

    private void FixedUpdate()
    {
        float Accel = groundCheck ? AirAccel : GroundAccel;


        if (groundCheck)
        {
            isJumping = jumpInput > 0f;
            if (isJumping)
            {
                rBody.velocity = new Vector2(rBody.velocity.x, jumpSpeed);
            }
        }
        else
        {
            if (horizontalInput > 0f)
            {
                //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, Accel), 0), ForceMode2D.Force);
                if(isHooked)
                    rBody.AddForce(new Vector2(Accel * 8, 0), ForceMode2D.Force);
                else
                    rBody.AddForce(new Vector2(Accel, 0), ForceMode2D.Force);
            }

            else if (horizontalInput < 0f)
            {
                //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0), ForceMode2D.Force);
                if (isHooked)
                    rBody.AddForce(new Vector2(-Accel * 8, 0), ForceMode2D.Force);
                else
                    rBody.AddForce(new Vector2(-Accel, 0), ForceMode2D.Force);
            }
            else
            {
                rBody.velocity = new Vector2(rBody.velocity.x * 0.99f, rBody.velocity.y);
            }
        }

        // clamp the velocity to something sane
        if (rBody.velocity.magnitude > 50)
            rBody.velocity = rBody.velocity.normalized * 50;

        if(groundCheck)
            rBody.velocity = new Vector2(rBody.velocity.x * 0.9f, rBody.velocity.y);
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
