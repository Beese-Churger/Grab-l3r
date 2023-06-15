using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rope2 : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject prefabRopeSeg;

    public HingeJoint2D top;

    public List<GameObject> ropeSegments = new List<GameObject>();
    public GameObject player;

    private TestRope testrope;
    private void Start()
    {
        //GenerateRope();
    }

    public void GenerateRope(Vector2 direction, int amount)
    {
        Rigidbody2D prevBod = hook;
        for(int i = 0; i < amount; i++)
        {
            GameObject newSeg = Instantiate(prefabRopeSeg/*, new Vector3(new Vector2(hook.position + 0.5f * -i * direction.normalized), 0*/);
            ropeSegments.Add(newSeg);
            newSeg.transform.parent = transform;
            newSeg.transform.position = hook.position + 0.5f * -i * direction.normalized;
            newSeg.transform.LookAt(prevBod.transform);
            newSeg.transform.rotation = Quaternion.Euler(new Vector3(newSeg.transform.rotation.x, 0, 0));
            //newSeg.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            HingeJoint2D hingeJoint = newSeg.GetComponent<HingeJoint2D>();
            hingeJoint.connectedBody = prevBod;

            if(i != 0)
            {
                DistanceJoint2D distanceJoint = newSeg.GetComponent<DistanceJoint2D>();
                distanceJoint.connectedBody = prevBod;
            }
            else
            {
                Destroy(newSeg.GetComponent<DistanceJoint2D>());
            }


            prevBod = newSeg.GetComponent<Rigidbody2D>();

            if(i==0)
            {
                top = hingeJoint;
            }
        }
        //player.GetComponent<HingeJoint2D>().connectedBody = prevBod;
    }

    public void addLink(Vector2 direction)
    {
        GameObject newLink = Instantiate(prefabRopeSeg);
        ropeSegments.Add(newLink);
        newLink.transform.parent = transform;
        newLink.transform.position = hook.position + 0.5f * -(ropeSegments.Count - 1) * direction.normalized;
        //newLink.transform.LookAt(ropeSegments[ropeSegments.Count - 2].transform);
        HingeJoint2D hingeJoint = newLink.GetComponent<HingeJoint2D>();
        hingeJoint.connectedBody = ropeSegments[ropeSegments.Count - 2].GetComponent<Rigidbody2D>();
        newLink.GetComponent<RopeSegment>().connectedBelow = player;
        //prevBod = newSeg.GetComponent<Rigidbody2D>();
    }

    public void removeLink()
    {
        //HingeJoint2D newTop = top.gameObject.GetComponent<RopeSegment>().connectedBelow.GetComponent<HingeJoint2D>();

        //link before last link;
        if (ropeSegments.Count > 1)
        {
            ropeSegments[ropeSegments.Count - 2].GetComponent<RopeSegment>().connectedBelow = player;
            Destroy(ropeSegments[ropeSegments.Count - 1]);
            ropeSegments.RemoveAt(ropeSegments.Count - 1);
            Debug.Log(ropeSegments.Count);
        }
        else
            return;

    }

    public void resetRope()
    {
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            Destroy(ropeSegments[i]);
        }
        ropeSegments.Clear();
        hook.gameObject.SetActive(false);
        player.GetComponent<HingeJoint2D>().enabled = false;
        hook.transform.SetParent(transform);
    }
}
