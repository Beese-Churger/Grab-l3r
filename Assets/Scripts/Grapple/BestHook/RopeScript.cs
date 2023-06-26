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

	int vertexCount = 1;
	public List<GameObject> Nodes = new List<GameObject>();
	private float lastInputTime;
	private float inputDelay = 0.05f;
	public float verticalInput;
	public bool done = false;
	public bool hooked = false;
	private Rigidbody2D rbody;
	public int climbspeed = 500;
	private float distancePlayer;
	float prevDistance;

	Vector2 prevVel;
	public bool cancelled = false;

	Vector3 oldPos;
	float totalDistance;
	float distanceThisFrame = 0f;
	public bool toDelete = false;
	float stopAt;
	bool down = false;
	void Start () 
	{

		cancelled = false;
		toDelete = false;
		lr = GetComponent<LineRenderer> ();

		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<throwhook>();

		lastNode = transform.gameObject;

		rbody = player.GetComponent<Rigidbody2D>();

		Nodes.Add (transform.gameObject);

		lastInputTime = Time.time;

		prevDistance = 0;
		prevVel = rbody.velocity;
		oldPos = transform.position;
		stopAt = 0;
	}
    void Update()
    {

		verticalInput = Input.GetAxis("Vertical");
		transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

		// get distance travelled from prev pos
		Vector3 distanceVector = player.transform.position - oldPos;
		distanceThisFrame = distanceVector.magnitude;
		totalDistance += distanceThisFrame;


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

			while (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
			{
				CreateNode(1);
			}


			//lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			//gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		}
		else
		{
			hooked = true;
		}
		if (hooked)
		{
			//changeMass();
			//if(rbody.velocity.y > 0)
			//         {
			//	Debug.Log("hit");
			//	rbody.AddForce(new Vector2(0, 0.3f), ForceMode2D.Force);
			//         }
			//transform.position = Vector2.MoveTowards(transform.position, Nodes[1].transform.position, speed);
			if (verticalInput >= 1f && vertexCount > 0)
			{
				for (int i = 0; i < vertexCount; ++i)
				{
					Nodes[i].GetComponent<Rigidbody2D>().mass = 1f;
				}
				//player.GetComponent<Rigidbody2D>().AddForce((Nodes[Nodes.Count - 2].transform.position - player.transform.position).normalized * 50f, ForceMode2D.Force);
				if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
				{
					rbody.AddForce((Nodes[0].transform.position - player.transform.position).normalized * 1.1f, ForceMode2D.Force);
					lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed;

					//player.transform.position = Vector2.MoveTowards(player.transform.position, (player.transform.position - lastNode.transform.position).normalized * distance + lastNode.transform.position, climbspeed);
				}

				else
					RemoveNode();


				//lastInputTime = Time.time;
			}


			if (lastInputTime + inputDelay < Time.time)
			{

				if (verticalInput < 0f && vertexCount < 50)
				{
					// prevent player from phasing into the ground
					// if (PlayerController.Instance.groundCheck)
					//	return;

					// prevent rope from extending if player is already on the floor
					//if (ropeHingeAnchor.transform.position.y <= transform.position.y)
					//	return;

					//ropeJoint.distance += Time.deltaTime * climbSpeed;
					CreateNode(1);
					cancelled = true;
					//lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastInputTime = Time.time;
					down = true;
				}

				if (playerScript.pulling && gameObject.GetComponent<SpringJoint2D>().connectedBody)
				{
					//gameObject.GetComponent<HingeJoint2D>().connectedBody = null;
					gameObject.GetComponent<SpringJoint2D>().connectedBody = null;
					gameObject.GetComponent<SpriteRenderer>().enabled = false;
					gameObject.GetComponent<Collider2D>().enabled = false;

				}
			}
	
			if(!cancelled)
            {
				stopAt = Nodes.Count - 1;
				Debug.Log(cancelled);
            }
			if (Nodes.Count > stopAt && !down)
            {
                Debug.Log("nodes: "+Nodes.Count+" |stopat: "+stopAt);

                //Debug.Log(vertexCount);
                if(Nodes[1].GetComponent<Rigidbody2D>().mass != 8f)
                {
                    for (int i = 0; i < vertexCount; ++i)
					{
						Nodes[i].GetComponent<Rigidbody2D>().mass = 8f;

					}
				}

				if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
				{
					lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed * player.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 10;
				}

				else
					RemoveNode();
			}

		}
		RenderLine();
		oldPos = transform.position;
	}
	void FixedUpdate () 
	{
		
	}
    private void OnDestroy()
    {
		//changeMass();
		//if (vertexCount > 10 && !cancelled)
		//{
		//	//Debug.Log(vertexCount);
		//	//Debug.Log(Nodes.Count);
		//	if (Nodes[1].GetComponent<Rigidbody2D>().mass != 8f)
		//	{
		//		for (int i = 0; i < vertexCount; ++i)
		//		{
		//			Nodes[i].GetComponent<Rigidbody2D>().mass = 8f;

		//		}
		//	}

		//	if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
		//	{
		//		lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed * player.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 10;

		//		//player.transform.position = Vector2.MoveTowards(player.transform.position, (player.transform.position - lastNode.transform.position).normalized * distance + lastNode.transform.position, climbspeed);
		//	}

		//	else
		//		RemoveNode();
		//}
	}

    private void LateUpdate()
    {
	
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

		//lr.SetPosition (i, player.transform.position);

	}


	private void CreateNode(int modifier)
	{

		Vector2 pos2Create = player.transform.position - lastNode.transform.position;
		pos2Create.Normalize();
		pos2Create *= distance * modifier;
		pos2Create += (Vector2)lastNode.transform.position;

		GameObject go = Instantiate (nodePrefab, pos2Create, Quaternion.identity);


		go.transform.SetParent (transform);

		//lastNode.GetComponent<HingeJoint2D> ().connectedBody = go.GetComponent<Rigidbody2D> ();
		lastNode.GetComponent<SpringJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
		lastNode.GetComponent<DistanceJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

		lastNode = go;

		// so rigidbodies dont lag behind
		//lastNode.GetComponent<Rigidbody2D>().AddForce(player.GetComponent<Rigidbody2D>().velocity, ForceMode2D.Force);

		
		Nodes.Add(lastNode);
		lastNode.name = "Link" + (Nodes.Count - 1);
		vertexCount++;

	}

	public void RemoveNode()
    {
		if (Nodes.Count <= 1)
			return;

		GameObject RemoveNode = Nodes[Nodes.Count - 1];
		//Vector2 position = RemoveNode.transform.position;
		Nodes.RemoveAt(Nodes.Count - 1);
		Destroy(RemoveNode);


		lastNode = Nodes[Nodes.Count - 1];
		//lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		//player.transform.position = position;

		vertexCount--;
    }


	public void TensionNode()
    {
		//get distance of player and how many nodes there are
		//if distance of player is shoerther than the amount of nodes, retract the rope
		//float distancePlayer = Vector2.Distance(playerScript.attachedTo.transform.position, player.transform.position);
		int index = 0;
		bool todel = false;
		for (int i = 0; i < Nodes.Count; ++i)
        {
			float nodeDistance = Vector2.Distance(player.transform.position, Nodes[i].transform.position);
			if (nodeDistance < lastNode.GetComponent<SpringJoint2D>().distance)
            {
				lastNode = Nodes[i];
				//lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
				index = i;
				todel = true;
			}
        }
		//if(todel)
  //      {
		//	for (int i = index + 1; i < Nodes.Count; ++i)
		//	{
		//		GameObject RemoveNode = Nodes[i];
		//		Nodes.RemoveAt(i);
		//		Destroy(RemoveNode);

		//	}
		//}
		if(Nodes.IndexOf(lastNode) > index )
        {
			//player.GetComponent<Rigidbody2D>().AddForce((Nodes[Nodes.Count - 2].transform.position - player.transform.position).normalized * 50f, ForceMode2D.Force);
			if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
			{
				lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * 10;
				//player.transform.position = Vector2.MoveTowards(player.transform.position, (player.transform.position - lastNode.transform.position).normalized * distance + lastNode.transform.position, climbspeed);
			}

			else
				RemoveNode();
		}


	}
	public void changeMass()
    {
        //if (Nodes[1].GetComponent<Rigidbody2D>().mass == 0.1f)
        //    return;
        //else
            for (int i = 1; i < Nodes.Count; ++i)
            {
				//Debug.Log(i);
				//Debug.Log(Nodes.Count);
				Nodes[i].GetComponent<Rigidbody2D>().mass = 0.1f;
            }
		verticalInput = -1f;
		cancelled = true;
    }


}
