
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeSystem : MonoBehaviour
{
    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public float climbSpeed = 3f;
    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    //public SpringJoint2D ropeJoint;
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public PlayerController playerMovement;
    private bool ropeAttached;
    private Vector2 playerPosition;
    private List<Vector2> ropePositions = new List<Vector2>();
    private float ropeMaxCastDistance = 20f;
    private Rigidbody2D ropeHingeAnchorRb;
    private bool distanceSet;
    private bool isColliding;
    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();
    private SpriteRenderer ropeHingeAnchorSprite;

    void Awake()
    {
        ropeJoint.enabled = false;
        playerPosition = transform.position;
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    // Figures out the closest Polygon collider vertex to a specified Raycast2D hit point in order to assist in 'rope wrapping'
    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        // Transform polygoncolliderpoints to world space (default is local)
        Dictionary<float,Vector2> distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 facingDirection = worldMousePosition - transform.position;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        playerPosition = transform.position;

        if (!ropeAttached)
        {
            SetCrosshairPosition(aimAngle);
            playerMovement.isSwinging = false;
        }
        else
        {
            playerMovement.isSwinging = true;
            playerMovement.ropeHook = ropePositions.Last();
            crosshairSprite.enabled = false;

            // Wrap rope around points of colliders if there are raycast collisions between player position and their closest current wrap around collider / angle point.
            if (ropePositions.Count > 0)
            {
                Vector2 lastRopePoint = ropePositions.Last();
                RaycastHit2D playerToCurrentNextHit = Physics2D.Raycast(playerPosition, (lastRopePoint - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint) - 0.1f, ropeLayerMask);
                if (playerToCurrentNextHit)
                {
                    PolygonCollider2D colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null)
                    {
                        var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);
                        if (wrapPointsLookup.ContainsKey(closestPointToHit))
                        {
                            // Reset the rope if it wraps around an 'already wrapped' position.
                            ResetRope();
                            return;
                        }

                        ropePositions.Add(closestPointToHit);
                        wrapPointsLookup.Add(closestPointToHit, 0);
                        distanceSet = false;
                    }
                }
            }
        }

        UpdateRopePositions();
        HandleRopeLength();
        HandleInput(aimDirection);
        HandleRopeUnwrap();
    }


    // Handles input within the RopeSystem component
    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            if (ropeAttached) return;
            ropeRenderer.enabled = true;

            //m_springJoint2D.connectedAnchor = grapplePoint;

            //Vector2 distanceVector = firePoint.position - gunHolder.position;

            //m_springJoint2D.distance = distanceVector.magnitude;
            //m_springJoint2D.frequency = launchSpeed;
            //m_springJoint2D.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);
            if (hit.collider != null)
            {
                ropeAttached = true;
                if (!ropePositions.Contains(hit.point))
                {
                    // Jump slightly to distance the player a little from the ground after grappling to something.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    wrapPointsLookup.Add(hit.point, 0);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;
                }
            }
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }

        if (Input.GetMouseButton(1))
        {
            ResetRope();
        }
    }

    // Resets the rope in terms of gameplay, visual, and supporting variable values.
    private void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePositions.Clear();
        wrapPointsLookup.Clear();
        ropeHingeAnchorSprite.enabled = false;
        //m_springJoint2D.enabled = false;
    }

    // Move the aiming crosshair based on aim angle
    private void SetCrosshairPosition(float aimAngle)
    {
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    // Retracts or extends the 'rope'
    private void HandleRopeLength()
    {
        
        if (Input.GetAxis("Vertical") >= 1f && ropeAttached && !isColliding)
        {
            ropeJoint.distance -= Time.deltaTime * climbSpeed;
        }
        else if (Input.GetAxis("Vertical") < 0f && ropeAttached)
        {
            // prevent player from phasing into the ground
            if (PlayerController.Instance.groundCheck)
                return;

            // prevent rope from extending if player is already on the floor
            if (ropeHingeAnchor.transform.position.y <= transform.position.y)
                return;
            ropeJoint.distance += Time.deltaTime * climbSpeed;
        }
    }

    // Handles updating of the rope hinge and anchor points based on objects the rope can wrap around. These must be PolygonCollider2D physics objects.
    private void UpdateRopePositions()
    {
        if (ropeAttached)
        {
            ropeRenderer.positionCount = ropePositions.Count + 1;

            for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
            {
                if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
                {
                    ropeRenderer.SetPosition(i, ropePositions[i]);

                    // Set the rope anchor to the 2nd to last rope position (where the current hinge/anchor should be) or if only 1 rope position then set that one to anchor point
                    if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                    {
                        if (ropePositions.Count == 1)
                        {
                            var ropePosition = ropePositions[ropePositions.Count - 1];
                            ropeHingeAnchorRb.transform.position = ropePosition;
                            if (!distanceSet)
                            {
                                ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                                distanceSet = true;
                            }
                        }
                        else
                        {
                            var ropePosition = ropePositions[ropePositions.Count - 1];
                            ropeHingeAnchorRb.transform.position = ropePosition;
                            if (!distanceSet)
                            {
                                ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                                distanceSet = true;
                            }
                        }
                    }
                    else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                    {
                        // if the line renderer position we're on is meant for the current anchor/hinge point...
                        var ropePosition = ropePositions.Last();
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                else
                {
                    // Player position
                    ropeRenderer.SetPosition(i, transform.position);
                }
            }
        }
    }

    private void HandleRopeUnwrap()
    {
        if (ropePositions.Count <= 1)
        {
            return;
        }

        // Hinge = next point up from the player position
        // Anchor = next point up from the Hinge
        // Hinge Angle = Angle between anchor and hinge
        // Player Angle = Angle between anchor and player

        // anchor Index is always 2 indexes away from player due to the hinge
        var anchorIndex = ropePositions.Count - 2;
        // most recent hinge created
        var hingeIndex = ropePositions.Count - 1;

        var anchorPosition = ropePositions[anchorIndex];
        var hingePosition = ropePositions[hingeIndex];
        var hingeDir = hingePosition - anchorPosition;
        var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
        var playerDir = playerPosition - anchorPosition;
        var playerAngle = Vector2.Angle(anchorPosition, playerDir);

        if (!wrapPointsLookup.ContainsKey(hingePosition))
        {
            Debug.LogError("We were not tracking hingePosition (" + hingePosition + ") in the look up dictionary.");
            return;
        }

        if (playerAngle < hingeAngle)
        {
            // unwrap
            if (wrapPointsLookup[hingePosition] == 1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            // 2
            wrapPointsLookup[hingePosition] = -1;
        }
        else
        {
            // 3
            if (wrapPointsLookup[hingePosition] == -1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            // 4
            wrapPointsLookup[hingePosition] = 1;
        }
    }

    private void UnwrapRopePosition(int anchorIndex, int hingeIndex)
    {
        // 1
        var newAnchorPosition = ropePositions[anchorIndex];
        wrapPointsLookup.Remove(ropePositions[hingeIndex]);
        ropePositions.RemoveAt(hingeIndex);

        // 2
        ropeHingeAnchorRb.transform.position = newAnchorPosition;
        distanceSet = false;

        // Set new rope distance joint distance for anchor position if not yet set.
        if (distanceSet)
        {
            return;
        }
        ropeJoint.distance = Vector2.Distance(transform.position, newAnchorPosition);
        distanceSet = true;
    }

    void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }
}
