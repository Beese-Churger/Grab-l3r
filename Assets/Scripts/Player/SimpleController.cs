using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private Rigidbody2D rBody;
    public float MAXSPEED = 100f;
    public float AirAccel = 10f;
    public float GroundAccel = 3f;
    private float jumpInput;
    private float horizontalInput;
    // Start is called before the first frame update
    void Start()
    {
  
        rBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        float Accel = AirAccel;

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


        if (Mathf.Abs(rBody.velocity.x) > MAXSPEED || Mathf.Abs(rBody.velocity.y) > MAXSPEED)
        {
            // clamp velocity:
            Vector3 newVelocity = rBody.velocity.normalized;
            newVelocity *= MAXSPEED;
            rBody.velocity = newVelocity;
        }
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
