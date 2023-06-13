using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope2 : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject prefabRopeSeg;
    public int numLinks = 5;

    public HingeJoint2D top;

    public List<GameObject> ropeSegments = new List<GameObject>();
    public GameObject player;

    private TestRope testrope;
    private void Start()
    {
        //GenerateRope();
    }

    public void GenerateRope(Vector3 direction, int amount)
    {
        Rigidbody2D prevBod = hook;
        for(int i = 0; i < amount; i++)
        {
            GameObject newSeg = Instantiate(prefabRopeSeg);
            ropeSegments.Add(newSeg);
            newSeg.transform.parent = transform;
            newSeg.transform.position = transform.position;
            HingeJoint2D hingeJoint = newSeg.GetComponent<HingeJoint2D>();
            hingeJoint.connectedBody = prevBod;
            prevBod = newSeg.GetComponent<Rigidbody2D>();

            if(i==0)
            {
                top = hingeJoint;
            }
        }
    }

    public void addLink()
    {
        GameObject newLink = Instantiate(prefabRopeSeg);
        ropeSegments.Add(newLink);
        newLink.transform.parent = transform;
        newLink.transform.position = transform.position;
        HingeJoint2D hingeJoint = newLink.GetComponent<HingeJoint2D>();
        hingeJoint.connectedBody = ropeSegments[ropeSegments.Count - 2].GetComponent<Rigidbody2D>();
        newLink.GetComponent<RopeSegment>().connectedBelow = player;
        //prevBod = newSeg.GetComponent<Rigidbody2D>();
    }

    public void removeLink()
    {
        //HingeJoint2D newTop = top.gameObject.GetComponent<RopeSegment>().connectedBelow.GetComponent<HingeJoint2D>();

        //link before last link;
        ropeSegments[ropeSegments.Count - 2].GetComponent<RopeSegment>().connectedBelow = player;
        Destroy(ropeSegments[ropeSegments.Count - 1]);
        ropeSegments.RemoveAt(ropeSegments.Count - 1);


    }

    public void resetRope()
    {
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            Destroy(ropeSegments[i]);
        }
        ropeSegments.Clear();
    }
}
