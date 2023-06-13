using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rope : MonoBehaviour
{
    public LayerMask ropeLayerMask;

    public Transform StartPoint;
    public Transform EndPoint;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.15f;
    private int segmentLength = 35;
    private float lineWidth = 0.1f;
    private bool Draw = false;
    private bool ropeAttached = false;
    private Vector3 worldMousePosition;
    private int indexMousePos;
    private bool moveToMouse = true;
    [SerializeField] private GameObject attachedTo;
    [SerializeField] private GameObject ropeHook;
    [SerializeField] Mesh meshToCollide;
    [SerializeField] private GameObject toCollide;
    [SerializeField] private EdgeCollider2D edgeCollider;
    Vector3 pointsas;
    public Vector2[] points2 = new Vector2[35];
    Mesh mesh;
    int i = 1;
    // Use this for initialization
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.useWorldSpace = false;
        Vector3 ropeStartPoint = StartPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
        mesh = gameObject.GetComponent<MeshFilter>().mesh;

    }

    // Update is called once per frame
    void Update()
    {
        if (Draw)
        {
            DrawRope();
            //DrawMesh();
        }


        worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 facingDirection = worldMousePosition - transform.position;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        float xStart = StartPoint.position.x;
        float currX = toCollide.transform.position.x;
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 20, ropeLayerMask);
            if (hit.collider != null)
            {
                AudioManager.Instance.PlaySFX("hook_attach");
                ropeAttached = true;
                EndPoint.position = hit.point;
                
                EndPoint.transform.parent = hit.transform;


                //if (!ropePositions.Contains(hit.point))
                //{


                //    //transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                //    ropePositions.Add(hit.point);
                //    wrapPointsLookup.Add(hit.point, 0);
                //    //ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                //    //ropeJoint.enabled = true;
                //    ropeHingeAnchorSprite.enabled = true;
                //    Grapple();
                //    attachedTo = hit.transform.gameObject;

                //    //hit.collider.transform.SetParent(ropeHingeAnchor.transform);
                //}
            }
        }
        float xEnd = EndPoint.position.x;
        float ratio = (currX - xStart) / (xEnd - xStart);
        if (ratio > 0)
        {
            indexMousePos = (int)(segmentLength * ratio);
            Debug.Log(indexMousePos);
        }
        if (Input.GetMouseButton(1))
        {
            ResetRope();
        }
    }

    private void FixedUpdate()
    {
        if(ropeAttached)
        {
            Draw = true;
            Simulate();
            getNewPositions();
            edgeCollider.points = points2;
            //meshCollider.sharedMesh = mesh;
            //getMesh(lineRenderer);

        }
    }
    void getNewPositions()
    {
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            pointsas = lineRenderer.GetPosition(i);
            points2[i] = new Vector2(pointsas.x, pointsas.y);
        }
    }
    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, -1.5f);

        for (int i = 1; i < segmentLength; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = StartPoint.position;
        ropeSegments[0] = firstSegment;

        //Constrant to Second Point 
        RopeSegment endSegment = ropeSegments[ropeSegments.Count - 1];
        endSegment.posNow = EndPoint.position;
        ropeSegments[ropeSegments.Count - 1] = endSegment;

        for (int i = 0; i < segmentLength - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
            if (moveToMouse && indexMousePos > 0 && indexMousePos < segmentLength - 1 && i == indexMousePos)
            {
                RopeSegment segment = ropeSegments[i];
                RopeSegment segment2 = ropeSegments[i + 1];
                segment.posNow = new Vector2(toCollide.transform.position.x, toCollide.transform.position.y);
                segment2.posNow = new Vector2(toCollide.transform.position.x, toCollide.transform.position.y);
                ropeSegments[i] = segment;
                ropeSegments[i + 1] = segment2;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    private void ResetRope()
    {
        Draw = false;
        ropeAttached = false;
    }

    private List<Vector3> points = new List<Vector3>();

    // Use this for initialization
    public void getMesh(LineRenderer lineRenderers)
    {
        //points.Clear();
        //lineRenderer = lineRenderers;
        //GameObject caret = null;
        //caret = new GameObject("lineRenderers");

        //Vector3 left, right; // A position to the left of the current lineRendererRenderer

        //// For all but the last point
        //for (var i = 0; i < lineRenderer.positionCount - 1; i++)
        //{
        //    caret.transform.position = lineRenderer.GetPosition(i);
        //    caret.transform.LookAt(lineRenderer.GetPosition(i + 1));
        //    right = caret.transform.position + transform.right * lineRenderer.startWidth / 2;
        //    left = caret.transform.position - transform.right * lineRenderer.startWidth / 2;
        //    points.Add(left);
        //    points.Add(right);
        //}

        //// Last point looks backwards and reverses
        //caret.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        //caret.transform.LookAt(lineRenderer.GetPosition(lineRenderer.positionCount - 2));
        //right = caret.transform.position + transform.right * lineRenderer.startWidth / 2;
        //left = caret.transform.position - transform.right * lineRenderer.startWidth / 2;
        //points.Add(left);
        //points.Add(right);
        //Destroy(caret);

        DrawMesh();
    }

    private void DrawMesh()
    {
        //Vector3[] verticies = new Vector3[points.Count];

        //for (int i = 0; i < verticies.Length; i++)
        //{
        //    verticies[i] = points[i];
        //}

        //int[] triangles = new int[((points.Count / 2) - 1) * 6];

        ////Works on lineRendererar patterns tn = bn+c
        //int position = 6;
        //for (int i = 0; i < (triangles.Length / 6); i++)
        //{
        //    triangles[i * position] = 2 * i;
        //    triangles[i * position + 3] = 2 * i;

        //    triangles[i * position + 1] = 2 * i + 3;
        //    triangles[i * position + 4] = (2 * i + 3) - 1;

        //    triangles[i * position + 2] = 2 * i + 1;
        //    triangles[i * position + 5] = (2 * i + 1) + 2;
        //}


        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        //mesh.vertices = verticies;
        //mesh.triangles = triangles;
        //mesh.RecalculateNormals();
        lineRenderer.BakeMesh(mesh, Camera.main);
    }
    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
