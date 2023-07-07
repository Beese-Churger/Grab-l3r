using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class RopeScript : MonoBehaviour
{

	[SerializeField] private InputActionReference movement;

	public Vector2 destiny;

	public float speed = 3;


	public float distance = 2;

	public GameObject nodePrefab;

	public GameObject player;
	public throwhook playerScript;

	public GameObject lastNode;


	public LineRenderer lr;

	private int vertexCount = 1;
	public List<GameObject> Nodes = new List<GameObject>();
	private float lastInputTime;
	private float inputDelay = 0.05f;
	private float verticalInput;
	private bool done = false;
	private bool hooked = false;
	private Rigidbody2D rbody;
	[SerializeField] private int climbspeed = 500;
	private float distancePlayer;
	private float prevDistance;

	private bool cancelled = false;

	private Vector3 oldPos;
	private float totalDistance;
	private float distanceThisFrame = 0f;
	private bool toDelete = false;
	private float stopAt;
	private bool down = false;
	private bool canHook;


	void Start()
	{

		cancelled = false;
		toDelete = false;
		lr = GetComponent<LineRenderer>();

		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<throwhook>();

		lastNode = transform.gameObject;

		rbody = player.GetComponent<Rigidbody2D>();

		Nodes.Add(transform.gameObject);

		lastInputTime = Time.time;

		prevDistance = 0;

		oldPos = transform.position;
		stopAt = 0;

	}
	void Update()
	{

		verticalInput = movement.action.ReadValue<Vector2>().y;
		//verticalInput = movement.action.ReadValue<Vector2>().y;

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
			lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
		}
		else
		{
			//Debug.Log(canHook);
			if (!canHook)
				Destroy(gameObject);
			hooked = true;
		}


		if (hooked)
		{
			if (verticalInput > 0f && vertexCount > 2)
			{
				for (int i = 0; i < vertexCount; ++i)
				{
					Nodes[i].GetComponent<Rigidbody2D>().mass = 1f;
				}

				if (playerScript.hookContext == throwhook.HookContext.HOOK_SMALL)
				{
					if (Nodes[1].GetComponent<SpringJoint2D>().distance > 0.005f)
					{
						//rbody.AddForce((Nodes[0].transform.position - player.transform.position).normalized * 1.1f, ForceMode2D.Force);
						Nodes[1].GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed;
					}
					else
						RemoveNode(1);
				}
				else
				{
					if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
					{
						//rbody.AddForce((Nodes[0].transform.position - player.transform.position).normalized * 1.1f, ForceMode2D.Force);
						lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed;
					}
					else
						RemoveNode(0);
				}
			}
			else
			{
				for (int i = 0; i < vertexCount; ++i)
				{
					Nodes[i].GetComponent<Rigidbody2D>().mass = 0.1f;
				}
			}


			if (lastInputTime + inputDelay < Time.time)
			{
				if (verticalInput < 0f && vertexCount < 50)
				{
					CreateNode(1);
					lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
					lastInputTime = Time.time;
					cancelled = true;
					down = true;
					changeMass();
				}

				if (playerScript.pulling && gameObject.GetComponent<SpringJoint2D>().connectedBody)
				{
					gameObject.GetComponent<SpringJoint2D>().connectedBody = null;
					gameObject.GetComponent<DistanceJoint2D>().connectedBody = null;
					gameObject.GetComponent<SpriteRenderer>().enabled = false;
					//gameObject.GetComponent<Collider2D>().enabled = false;

				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				changeMass();
				cancelled = true;
				down = true;
			}

			if (!cancelled)
			{
				if (Nodes.Count > 2)
					stopAt = Nodes.Count - 1;
			}

			if (playerScript.hookContext == throwhook.HookContext.HOOK_SMALL)
			{
				if (Nodes.Count > stopAt && !down)
				{
					//Debug.Log("nodes: "+Nodes.Count+" |stopat: "+stopAt);
					//if (Nodes[1].GetComponent<Rigidbody2D>().mass != Nodes.Count * 0.3f)
					//{
					for (int i = 0; i < vertexCount; ++i)
					{
						Nodes[i].GetComponent<Rigidbody2D>().mass = Nodes.Count * 0.1f;

					}
					//}

					if (Nodes[1].GetComponent<SpringJoint2D>().distance > 0.005f)
					{
						Nodes[1].GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed * playerScript.attachedTo.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 10;
					}
					else
						RemoveNode(1);
				}
			}
			else
			{
				if (Nodes.Count > stopAt && !down)
				{
					//Debug.Log("nodes: "+Nodes.Count+" |stopat: "+stopAt);
					//if (Nodes[1].GetComponent<Rigidbody2D>().mass != Nodes.Count * 0.3f)
					//{
					for (int i = 0; i < vertexCount; ++i)
					{
						Nodes[i].GetComponent<Rigidbody2D>().mass = Nodes.Count * 0.1f;

					}
					//}

					if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
					{
						lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed * player.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 10;
					}
					else
						RemoveNode(0);
				}
			}
		}
		RenderLine();
		oldPos = transform.position;
	}

	private void RenderLine()
	{
		lr.SetVertexCount(vertexCount);

		int i = 0;
		if (hooked && playerScript.pulling)
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

		GameObject go = Instantiate(nodePrefab, pos2Create, Quaternion.identity);
		go.transform.SetParent(transform);

		lastNode.GetComponent<SpringJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
		lastNode.GetComponent<DistanceJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

		lastNode = go;

		Nodes.Add(lastNode);
		lastNode.name = "Link" + (Nodes.Count - 1);
		vertexCount++;

	}

	public void RemoveNode(int first)
	{
		if (Nodes.Count <= 1)
			return;

		// if first = 0, remove from last, if 1 remove from first

		if (first == 0)
		{
			GameObject RemoveNode = Nodes[Nodes.Count - 1];
			Nodes.RemoveAt(Nodes.Count - 1);
			Destroy(RemoveNode);


			lastNode = Nodes[Nodes.Count - 1];
			lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();

		}
		else
		{
			GameObject RemoveNode = Nodes[1];
			Nodes.RemoveAt(1);
			Destroy(RemoveNode);


			GameObject firstNode = Nodes[1];
			if (Nodes.Count > 3)
			{
				playerScript.attachedTo.GetComponent<DistanceJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
				playerScript.attachedTo.GetComponent<SpringJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
			}
			else
			{
				playerScript.attachedTo.GetComponent<DistanceJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
				playerScript.attachedTo.GetComponent<SpringJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
			}
		}

		vertexCount--;
	}


	public void TensionNode()
	{
		//get distance of player and how many nodes there are
		//if distance of player is shoerther than the amount of nodes, retract the rope
		int index = 0;
		bool todel = false;
		for (int i = 0; i < Nodes.Count; ++i)
		{
			float nodeDistance = Vector2.Distance(player.transform.position, Nodes[i].transform.position);
			if (nodeDistance < lastNode.GetComponent<SpringJoint2D>().distance)
			{
				lastNode = Nodes[i];
				index = i;
				todel = true;
			}
		}
		if (Nodes.IndexOf(lastNode) > index)
		{
			if (lastNode.GetComponent<SpringJoint2D>().distance > 0.005f)
			{
				lastNode.GetComponent<SpringJoint2D>().distance -= Time.deltaTime * 10;
			}

			else
				RemoveNode(0);
		}


	}
	public void changeMass()
	{
		for (int i = 1; i < Nodes.Count; ++i)
		{
			Nodes[i].GetComponent<Rigidbody2D>().mass = 0.1f;
		}
	}

	public void SetCanHook(bool _hook)
	{
		canHook = _hook;
	}

	public bool isHooked()
	{
		return hooked;
	}
}
