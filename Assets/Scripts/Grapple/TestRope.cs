using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRope : MonoBehaviour
{
    public int maxLinks = 15;
    public int numLinks;
    public LayerMask ropeLayerMask;
    [SerializeField] private rope2 rope;
    public HingeJoint2D hj;
    public GameObject hook;
    public GameObject attachedTo;
    public GameObject pivot;
    public HingeJoint2D pivothj;

    private float inputDelay = 0.3f;
    private float lastInputTime;

    public float MAXSPEED = 20f;
    public float AirAccel = 1.5f;
    public float GroundAccel = 3f;

    private Rigidbody2D rBody;
    private bool isJumping;
    private float jumpInput;
    private float horizontalInput;
    private float verticalInput;
    private bool ropeAttached = false;
    public InputActionReference pointer;
    private bool groundCheck;
    private float halfHeight;
    private float Accel;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;

    Vector3 aimDirection;
    Vector3 worldMousePosition;
    void Awake()
    {
        //rope = transform.parent.GetComponent<rope2>();
  
        lastInputTime = Time.time;
        rBody = GetComponent<Rigidbody2D>();
        hook.SetActive(false);
        hj.enabled = false;
        halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        //pivot.SetActive(false);
    }

    void Update()
    {
        numLinks = rope.ropeSegments.Count;
        
        worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointer.action.ReadValue<Vector2>().x, pointer.action.ReadValue<Vector2>().y, 0f));
        Vector3 facingDirection = worldMousePosition - transform.position;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

       

        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        if (!ropeAttached)
        {
            SetCrosshairPosition(aimAngle);
            //playerMovement.isSwinging = false;
        }

        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
        Accel = groundCheck ? GroundAccel : AirAccel;

        handleInput(aimDirection);
        handleKBInput();
    }

    private void handleKBInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (rope.ropeSegments.Count > 0)
            pivothj.connectedBody = rope.ropeSegments[rope.ropeSegments.Count - 1].GetComponent<Rigidbody2D>();

        if (lastInputTime + inputDelay < Time.time)
        {
            if (verticalInput >= 1f && numLinks > 0)
            {
                rope.removeLink();
                numLinks--;
                if (rope.ropeSegments.Count > 0)
                { 
                    pivothj.connectedBody = rope.ropeSegments[rope.ropeSegments.Count - 1].GetComponent<Rigidbody2D>();
                }
                lastInputTime = Time.time;
            }
            else if (verticalInput < 0f && numLinks < 15)
            {
                Vector3 swingDirection = rope.ropeSegments[rope.ropeSegments.Count - 1].transform.position - transform.position;
                rope.addLink(swingDirection.normalized);
                numLinks++;
                lastInputTime = Time.time;
            }
        }

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

    private void handleInput(Vector3 aimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            if (ropeAttached) return;


            hook.SetActive(true);
            hj.enabled = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, maxLinks, ropeLayerMask);
            if (hit.collider != null)
            {
                //pivot.SetActive(true);
                attachedTo = hit.transform.gameObject;
                //AudioManager.Instance.PlaySFX("hook_attach");
                ropeAttached = true;
                hook.transform.position = hit.point;
                hook.transform.SetParent(attachedTo.transform);
                
     
                if(hit.transform.GetComponent<Rigidbody2D>() != null)
                {
                    HingeJoint2D toPull = attachedTo.AddComponent<HingeJoint2D>();
                    toPull.anchor = hook.transform.localPosition;
                    toPull.connectedBody = hook.GetComponent<Rigidbody2D>();
                    hook.GetComponent<HingeJoint2D>().connectedBody = hit.transform.GetComponent<Rigidbody2D>();
                }
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
            ropeAttached = false;
            hj.enabled = false;
            pivothj.connectedBody = rBody;
            //pivot.SetActive(false);
            Destroy(attachedTo.GetComponent<HingeJoint2D>());
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
        //Debug.Log(RopeLinkLength);
        float DistanceBetweenPoints = Vector2.Distance(transform.position, hook.transform.position);
        //Debug.Log(DistanceBetweenPoints);
        int AmountOfLinksNeeded = Mathf.RoundToInt(DistanceBetweenPoints / RopeLinkLength);

        //Debug.Log(AmountOfLinksNeeded);
        if(AmountOfLinksNeeded <= maxLinks)
        {
            rope.GenerateRope(aimDirection, AmountOfLinksNeeded);
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
