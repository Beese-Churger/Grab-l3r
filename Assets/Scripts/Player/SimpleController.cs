using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private Rigidbody2D rBody;
    public float MAXSPEED = 100f;
    public float AirAccel = 3f;
    public float GroundAccel = 3f;
    private float jumpInput;
    private float horizontalInput;
    public bool groundCheck;
    // Start is called before the first frame update
    void Start()
    {
  
        rBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);

        horizontalInput = Input.GetAxis("Horizontal");

        float Accel = groundCheck ? AirAccel : GroundAccel;


        if (horizontalInput > 0f)
        {
            //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, Accel), 0), ForceMode2D.Force);
            rBody.AddForce(new Vector2(Accel, 0), ForceMode2D.Force);
        }

        else if (horizontalInput < 0f)
        {
            //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0), ForceMode2D.Force);
            rBody.AddForce(new Vector2(-Accel, 0), ForceMode2D.Force);
        }
        
       


        // clamp the velocity to something sane
        if (rBody.velocity.magnitude > 100)
            rBody.velocity = rBody.velocity.normalized * 100;
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
