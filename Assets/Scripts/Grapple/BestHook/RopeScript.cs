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

	private bool cancelled = false;

	private float stopAt;
	private bool down = false;
	private bool canHook;
	private bool render = true;


	void Start()
	{

		cancelled = false;

		lr = GetComponent<LineRenderer>();

		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<throwhook>();

		lastNode = transform.gameObject;

		rbody = player.GetComponent<Rigidbody2D>();

		Nodes.Add(transform.gameObject);

		lastInputTime = Time.time;

		stopAt = 0;

	}
	void Update()
	{
		verticalInput = movement.action.ReadValue<Vector2>().y;

		transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

		if ((Vector2)transform.position != destiny)
		{
			lr.positionCount = 2;
			lr.SetPosition(0, transform.position);
			lr.SetPosition(1, player.transform.position);
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
			if (!canHook)
            {
				AudioManager.Instance.PlaySFX("hook_no_attach", transform.position);
				Destroy(gameObject);
				return;
			}
			if(!hooked)
				AudioManager.Instance.PlaySFX("hook_attach", transform.position);
			hooked = true;
		}

		if (hooked)
		{
			if (verticalInput > 0f && vertexCount > 2 && cancelled) // dont allow to go up while rope is auto tensioning
			{
				if (playerScript.hookContext == throwhook.HookContext.HOOK_SMALL)
				{
					Vector2 pos2Create = (playerScript.attachedTo.transform.position - Nodes[1].transform.position);
					pos2Create.Normalize();
					pos2Create *= Nodes[1].GetComponent<DistanceJoint2D>().distance;
					pos2Create += (Vector2)Nodes[1].transform.position;

					if (Vector2.Distance(playerScript.attachedTo.transform.position, pos2Create) < 0.45f)
                    {
						PullTowardsPlayer();
						playerScript.attachedTo.transform.position = Vector2.MoveTowards(playerScript.attachedTo.transform.position, pos2Create, Time.deltaTime * 10);
					}
				}
				else
				{
					Vector2 pos2Create = -(player.transform.position - lastNode.transform.position);
					pos2Create.Normalize();
					pos2Create *= lastNode.GetComponent<DistanceJoint2D>().distance;
					pos2Create += (Vector2)lastNode.transform.position;

					if (Vector2.Distance(player.transform.position, pos2Create) <= 0.45f)
                    {
						PullPlayerTowards();
						player.transform.position = Vector2.MoveTowards(player.transform.position, pos2Create, Time.deltaTime * 10);
					}
				}
			}
			
			if (lastInputTime + inputDelay < Time.time)
			{
				ExtendRope();
			}

			TensionNode();
			RenderLine();
		}
	}

    private void RenderLine()
	{
		if (!render)
			return;

		lr.positionCount = Nodes.Count + 1;

		int i = 0;
		if (hooked && playerScript.pulling && playerScript.attachedTo != null)
			lr.SetPosition(0, playerScript.attachedTo.transform.position);
		else
			lr.SetPosition(0, Nodes[1].transform.position);

		for (i = 1; i < Nodes.Count; i++)
		{
			lr.SetPosition(i, Nodes[i].transform.position);
		}

		lr.SetPosition(lr.positionCount - 1, player.transform.position);
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
			playerScript.attachedTo.GetComponent<DistanceJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
			playerScript.attachedTo.GetComponent<SpringJoint2D>().connectedBody = firstNode.GetComponent<Rigidbody2D>();
		}
		vertexCount--;
	}


	public void TensionNode()
	{
		if (!cancelled || !playerScript.ropeActive)
		{
			if (Nodes.Count > 2)
				stopAt = Nodes.Count - 1;
		}


		if (playerScript.hookContext == throwhook.HookContext.HOOK_SMALL)
		{
			Vector2 pos2Create = (playerScript.attachedTo.transform.position - Nodes[1].transform.position);
			pos2Create.Normalize();
			pos2Create *= Nodes[1].GetComponent<DistanceJoint2D>().distance;
			pos2Create += (Vector2)Nodes[1].transform.position;

			if (Vector2.Distance(playerScript.attachedTo.transform.position, pos2Create) >= 0.45f)
				return;

			if (Nodes.Count > stopAt && !down)
			{
				PullTowardsPlayer();
				playerScript.attachedTo.transform.position = Vector2.MoveTowards(playerScript.attachedTo.transform.position, pos2Create, Time.deltaTime * 10);
			}
		}
		else
		{
			Vector2 pos2Create = -(player.transform.position - lastNode.transform.position);
			pos2Create.Normalize();
			pos2Create *= lastNode.GetComponent<DistanceJoint2D>().distance;
			pos2Create += (Vector2)lastNode.transform.position;

			if (Vector2.Distance(player.transform.position, pos2Create) >= 0.45f)
				return;

			if (Nodes.Count > stopAt && !down)
			{
				PullPlayerTowards();
				player.transform.position = Vector2.MoveTowards(player.transform.position, pos2Create, Time.deltaTime * 10);
			}
		}
	}

	void PullTowardsPlayer()
	{
		if (Nodes[1].GetComponent<SpringJoint2D>().distance > 0.005f)
		{
			Nodes[1].GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed;
		}
		else
			RemoveNode(1);

		if (Nodes[1].GetComponent<SpringJoint2D>().distance > 0.005f)
		{
			Nodes[1].GetComponent<SpringJoint2D>().distance -= Time.deltaTime * climbspeed * playerScript.attachedTo.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 20;
		}
		else
			RemoveNode(1);
	}

	void PullPlayerTowards()
    {
		// I have no idea y having these 2 together makes going up the rops smoother but it works so im not complaining

		if (lastNode.GetComponent<DistanceJoint2D>().distance > 0.005f)
		{
			lastNode.GetComponent<DistanceJoint2D>().distance -= Time.deltaTime * climbspeed;
		}
		else
			RemoveNode(0);

		if (lastNode.GetComponent<DistanceJoint2D>().distance > 0.005f)
		{
			lastNode.GetComponent<DistanceJoint2D>().distance -= Time.deltaTime * climbspeed * player.GetComponent<Rigidbody2D>().velocity.magnitude * vertexCount * 20;
		}
		else
			RemoveNode(0);

	}

	void ExtendRope()
    {
		if (verticalInput < 0f && vertexCount < 50)
		{
			CreateNode(1);
			lastNode.GetComponent<SpringJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastNode.GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
			lastInputTime = Time.time;
			cancelled = true;
			down = true;
		}

		if (playerScript.pulling && gameObject.GetComponent<SpringJoint2D>().connectedBody)
		{
			gameObject.GetComponent<SpringJoint2D>().connectedBody = null;
			gameObject.GetComponent<DistanceJoint2D>().connectedBody = null;
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
			//gameObject.GetComponent<Collider2D>().enabled = false;
		}
	}
	public void SetCanHook(bool _hook)
	{
		canHook = _hook;
	}
}
