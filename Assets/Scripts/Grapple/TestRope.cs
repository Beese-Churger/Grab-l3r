using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRope : MonoBehaviour
{
    public int maxLinks = 15;
    private int numLinks;
    public LayerMask ropeLayerMask;
    [SerializeField] private rope2 rope;
    public HingeJoint2D hj;
    public GameObject hook;

    private float inputDelay = 0.3f;
    private float lastInputTime;

    public float MAXSPEED = 20f;
    public float AirAccel = 0.5f;
    public float GroundAccel = 3f;

    private Rigidbody2D rBody;
    private bool isJumping;
    private float jumpInput;
    private float horizontalInput;
    private float verticalInput;
    private bool ropeAttached = false;
    public InputActionReference pointer;

    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    void Awake()
    {
        //rope = transform.parent.GetComponent<rope2>();
        numLinks = rope.numLinks;
        lastInputTime = Time.time;
        rBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointer.action.ReadValue<Vector2>().x, pointer.action.ReadValue<Vector2>().y, 0f));
        Vector3 facingDirection = worldMousePosition - transform.position;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        if (!ropeAttached)
        {
            SetCrosshairPosition(aimAngle);
            //playerMovement.isSwinging = false;
        }

        handleKBInput();
    }

    private void handleKBInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (rope.ropeSegments.Count > 0)
            hj.connectedBody = rope.ropeSegments[rope.ropeSegments.Count - 1].GetComponent<Rigidbody2D>();

        if (lastInputTime + inputDelay < Time.time)
        {
            if (verticalInput >= 1f && numLinks > 0)
            {
                rope.removeLink();
                numLinks--;
                if (rope.ropeSegments.Count > 0)
                    hj.connectedBody = rope.ropeSegments[rope.ropeSegments.Count - 1].GetComponent<Rigidbody2D>();
                lastInputTime = Time.time;
            }
            else if (verticalInput < 0f && numLinks < 15)
            {
                rope.addLink();
                numLinks++;
                lastInputTime = Time.time;
            }
        }

        float Accel = AirAccel;
        if (horizontalInput > 0f)
        {
            //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, Accel), 0), ForceMode2D.Force);
            rBody.AddForce(new Vector2(1.5f, 0), ForceMode2D.Force);
        }

        else if (horizontalInput < 0f)
        {
            //rBody.AddForce(new Vector2(SaturatedAdd(-MAXSPEED, MAXSPEED, rBody.velocity.x, -Accel), 0), ForceMode2D.Force);
            rBody.AddForce(new Vector2(-1.5f, 0), ForceMode2D.Force);
        }


        if (Mathf.Abs(rBody.velocity.x) > MAXSPEED || Mathf.Abs(rBody.velocity.y) > MAXSPEED)
        {
            // clamp velocity:
            Vector3 newVelocity = rBody.velocity.normalized;
            newVelocity *= MAXSPEED;
            rBody.velocity = newVelocity;
        }
    }

    private void handleInput(Vector3 aimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            if (ropeAttached) return;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, maxLinks, ropeLayerMask);
            if (hit.collider != null)
            {
                AudioManager.Instance.PlaySFX("hook_attach");
                ropeAttached = true;
                hook.transform.position = hit.point;
                Grapple();
            }
            else
            {
                ropeAttached = false;
                hj.enabled = false;
            }
        }

        if (Input.GetMouseButton(1))
        {
            rope.resetRope();
        }
    }
    private void SetCrosshairPosition(float aimAngle)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        float x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        float y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        Vector3 crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    private void Grapple()
    {
        float RopeLinkLength = rope.prefabRopeSeg.transform.localScale.y;
        float DistanceBetweenPoints = Vector2.Distance(transform.position, hook.transform.position);
        int AmountOfLinksNeeded = Mathf.RoundToInt(DistanceBetweenPoints / RopeLinkLength);

        if(AmountOfLinksNeeded <= maxLinks)
        {
            rope.GenerateRope(Vector3.zero, AmountOfLinksNeeded);
        }
    }
    //// increases or decreases speed at first, but only up to a maximum (saturation) level
    //private float SaturatedAdd(float Min, float Max, float Current, float Modifier)
    //{
    //    if (Modifier < 0)
    //    {
    //        if (Current < Min)
    //            return Current;
    //        Current += Modifier;
    //        if (Current < Min)
    //            Current = Min;
    //        return Current;
    //    }
    //    else
    //    {
    //        if (Current > Max)
    //            return Current;
    //        Current += Modifier;
    //        if (Current > Max)
    //            Current = Max;
    //        return Current;
    //    }
    //}
}
