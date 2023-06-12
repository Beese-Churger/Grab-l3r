using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RopePhysics : MonoBehaviour
{
    public Transform Player;
    public GameObject RopeLinkPrefab;
    public Transform RopeParent;
    public Transform RopeAnchor;
    [SerializeField] public GameObject RopeHook;
    [SerializeField] private GameObject attachedTo;
    public GameObject JointAnchor;
    public List<GameObject> RopeLinks;
    public float RopeLinkLength;
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public float DistanceBetweenPoints;
    public float AngleBetweenPoints;
    public Vector3 Offset;
    public int AmountOfLinksNeeded;
    public LayerMask ropeLayerMask;
    public DistanceJoint2D distanceJoint;
    public GameObject Pivot;

    public HingeJoint2D joint2;
    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        //distanceJoint = this.GetComponent<DistanceJoint2D>();
    }
    void GenerateRope()
    {
        Vector3 facingDirection = -EndPoint-StartPoint;
        DistanceBetweenPoints = Vector3.Distance(StartPoint, EndPoint);
        GetAngle();
        //AngleBetweenPoints = Vector3.Angle(StartPoint, EndPoint);
        AmountOfLinksNeeded = Mathf.RoundToInt(DistanceBetweenPoints / RopeLinkLength);
        //FIX! Offset MUST take into account angle
        //Offset = new Vector3((DistanceBetweenPoints / AmountOfLinksNeeded) * Mathf.Cos(AngleBetweenPoints), ((DistanceBetweenPoints / AmountOfLinksNeeded) * Mathf.Sin(AngleBetweenPoints)), 0);
        if (RopeLinks.Count <= 0)
        {
            //ar ropelink = Instantiate(RopeLinkPrefab, StartPoint, Quaternion.Euler(facingDirection), RopeParent);

            JointAnchor.GetComponent<SpriteRenderer>().color = Color.green;
            joint2 = JointAnchor.AddComponent<HingeJoint2D>();
            joint2.anchor = new Vector2(0,-0.5f);
            joint2.connectedAnchor = new Vector2(0, 0.25f);
            //ropelink.transform.name = "Link0";
            JointAnchor.SetActive(true);
            joint2.connectedBody = Player.GetComponent<Rigidbody2D>();
            RopeLinks.Add(JointAnchor);
        }
        for (int i = 1; i < AmountOfLinksNeeded; i++)
        {
            Debug.Log("hit");
            //Offset = new Vector3(RopeLinks.Last().transform.position.x + (RopeLinkLength * 2) * Mathf.Cos(RopeLinks.Last().transform.rotation.eulerAngles.z),
            //RopeLinks.Last().transform.position.y + (RopeLinkLength* 2) * Mathf.Sin(RopeLinks.Last().transform.rotation.eulerAngles.z), 0);
            var ropelink = Instantiate(RopeLinkPrefab, RopeLinks.Last().transform.position + (facingDirection * (0.125f)), Quaternion.Euler(transform.up), RopeParent);
            ropelink.transform.LookAt(RopeHook.transform);
            Debug.Log("Amount of rope links: " + RopeLinks.Count);
            if (i != (AmountOfLinksNeeded - 1))
                RopeLinks[RopeLinks.Count - 1].GetComponent<HingeJoint2D>().connectedBody = ropelink.GetComponent<Rigidbody2D>();
            else
            {
                //ropelink.GetComponent<Rigidbody2D>().isKinematic = true;
                RopeLinks[RopeLinks.Count - 1].GetComponent<HingeJoint2D>().connectedBody = Player.GetComponent<Rigidbody2D>();
                //distanceJoint.enabled = true;
                //distanceJoint.connectedBody = Player.GetComponent<Rigidbody2D>();
                //distanceJoint.distance += 1;
                ropelink.GetComponent<SpriteRenderer>().color = Color.red;
            }
            ropelink.transform.name = "Link" + i;
            RopeLinks.Add(ropelink);
        }
    }

    void Update()
    {

        //JointAnchor.transform.position = RopeHook.transform.position;
        RopeHook.transform.position = RopeAnchor.transform.position;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 facingDirection = worldMousePosition - transform.position;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        StartPoint = Pivot.transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(StartPoint, aimDirection, ropeLayerMask);
            if (hit.collider != null)
            {
                RopeAnchor.transform.position = hit.point;
                attachedTo = hit.transform.gameObject;
                RopeLinks.Clear();
                RopeAnchor.transform.SetParent(attachedTo.transform);
                StartPoint =RopeHook.transform.position;
                EndPoint = Player.position;
            }
            GenerateRope();
            //Debug.Log(hit.transform.gameObject);
            //Debug.Log("<color=green>Generating rope between</color> " + StartPoint + " & " + EndPoint);
            //Debug.Log("<color=green>Rope start point: </color>" + StartPoint);
            //Debug.Log("<color=red>Rope end point: </color>" + EndPoint);
        }
    }

    void GetAngle()
    {
        Vector3 facingDirection = StartPoint - EndPoint;
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        AngleBetweenPoints = aimAngle;
    }
}
