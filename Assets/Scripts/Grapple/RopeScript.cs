using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RopeScript : MonoBehaviour {

	public Vector2 destiny;

	public float speed= 3;


	public float distance = 2;

	public GameObject nodePrefab;

	public GameObject player;
	public throwhook playerScript;

	public GameObject lastNode;


	public LineRenderer lr;

	int vertexCount=2;
	public List<GameObject> Nodes = new List<GameObject>();
	private float lastInputTime;
	private float inputDelay = 0.05f;
	private float verticalInput;
	public bool done = false;
	public bool hooked = false;
	void Start () {
	

		lr = GetComponent<LineRenderer> ();

		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<throwhook>();

		lastNode = transform.gameObject;


		Nodes.Add (transform.gameObject);

		lastInputTime = Time.time;

	}
	
	void Update () {
	

		transform.position = Vector2.MoveTowards (transform.position,destiny,speed);


		verticalInput = Input.GetAxis("Vertical");

		if ((Vector2)transform.position != destiny)
		{
			if (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
			{
				CreateNode(1);
			}		
		} 


		else if (done == false) 
		{

			done = true;

			while(Vector2.Distance (player.transform.position, lastNode.transform.position) > distance)
			{
				CreateNode (1);
			}


			lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			//gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		}
		else
		{
			hooked = true;
		}
		if (hooked)
        {
			//transform.position = Vector2.MoveTowards(transform.position, Nodes[1].transform.position, speed);
			if (lastInputTime + inputDelay < Time.time)
			{
				if (verticalInput >= 1f && vertexCount > 0)
				{
					//ropeJoint.distance -= Time.deltaTime * climbSpeed;
					RemoveNode();


					lastInputTime = Time.time;
				}
				else if (verticalInput < 0f && vertexCount < 50)
				{
					// prevent player from phasing into the ground
					// if (PlayerController.Instance.groundCheck)
					//	return;

					// prevent rope from extending if player is already on the floor
					//if (ropeHingeAnchor.transform.position.y <= transform.position.y)
					//	return;

					//ropeJoint.distance += Time.deltaTime * climbSpeed;
					CreateNode(1);

					lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();

					lastInputTime = Time.time;
				}

				if(playerScript.pulling && gameObject.GetComponent<HingeJoint2D>().connectedBody)
                {
					gameObject.GetComponent<HingeJoint2D>().connectedBody = null;
					gameObject.GetComponent<DistanceJoint2D>().connectedBody = null;
					gameObject.GetComponent<SpriteRenderer>().enabled = false;
					gameObject.GetComponent<Collider2D>().enabled = false;

				}
			}
			
		}
		RenderLine ();
	}


	private void RenderLine()
	{

        lr.SetVertexCount(vertexCount);
	
		int i = 0;
		if(hooked && playerScript.pulling)
			lr.SetPosition(i, playerScript.attachedTo.transform.position);
		else
			lr.SetPosition(i, Nodes[i].transform.position);

		for (i = 1; i < Nodes.Count; i++) 
		{

			lr.SetPosition(i, Nodes[i].transform.position);

		}

		lr.SetPosition (i, player.transform.position);

	}


	private void CreateNode(int modifier)
	{

		Vector2 pos2Create = player.transform.position - lastNode.transform.position;
		pos2Create.Normalize ();
		pos2Create *= distance * modifier;
		pos2Create += (Vector2)lastNode.transform.position;

		GameObject go = Instantiate (nodePrefab, pos2Create, Quaternion.identity);


		go.transform.SetParent (transform);

		lastNode.GetComponent<HingeJoint2D> ().connectedBody = go.GetComponent<Rigidbody2D> ();
		lastNode.GetComponent<DistanceJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

		lastNode = go;

		Nodes.Add(lastNode);
		lastNode.name = "Link" + (Nodes.Count - 1);
		vertexCount++;

	}

	public void RemoveNode()
    {
		if (Nodes.Count <= 1)
			return;

		GameObject RemoveNode = Nodes[Nodes.Count - 1];
		Vector2 position = RemoveNode.transform.position;
		Nodes.RemoveAt(Nodes.Count - 1);
		Destroy(RemoveNode);


		lastNode = Nodes[Nodes.Count - 1];
		lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		//player.transform.position = position;
		player.GetComponent<Rigidbody2D>().AddForce(/*(lastNode.transform.position - player.transform.position).normalized*/transform.up * 5f, ForceMode2D.Force);
		vertexCount--;
    }

}
