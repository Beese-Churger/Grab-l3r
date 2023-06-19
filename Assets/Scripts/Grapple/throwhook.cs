using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class throwhook : MonoBehaviour {


	public GameObject hook;

	public InputActionReference pointer;

	public bool ropeActive;

	public int maxLinks = 15;

	public LayerMask ropeLayerMask;

	public GameObject attachedTo;

	public RopeScript ropeScript;

	GameObject curHook;

	bool change = false;

	public bool pulling = false;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointer.action.ReadValue<Vector2>().x, pointer.action.ReadValue<Vector2>().y, 0f));
		Vector3 facingDirection = worldMousePosition - transform.position;
		float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);



		if (aimAngle < 0f)
		{
			aimAngle = Mathf.PI * 2 + aimAngle;
		}

		Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

		if (Input.GetMouseButtonDown (0)) {


			if (ropeActive == false) {

				RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, maxLinks, ropeLayerMask);
				if (hit.collider != null)
				{
					attachedTo = hit.transform.gameObject;
					AudioManager.Instance.PlaySFX("hook_attach");
					curHook = Instantiate(hook, transform.position, Quaternion.identity);
					//hook.transform.SetParent(attachedTo.transform);
					curHook.GetComponent<RopeScript>().destiny = hit.point;

					if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
					{
						//Debug.Log(hit.transform.gameObject.name);
						change = true;
						
						//hook.GetComponent<HingeJoint2D>().connectedBody = hit.transform.GetComponent<Rigidbody2D>();
						//hook.GetComponent<DistanceJoint2D>().connectedBody = hit.transform.GetComponent<Rigidbody2D>();
					}
					ropeActive = true;
				}
			}
			else
			{

				//delete rope

				Destroy(curHook);

				Destroy(attachedTo.GetComponent<HingeJoint2D>());
				Destroy(attachedTo.GetComponent<DistanceJoint2D>());
				ropeActive = false;
				change = false;
				pulling = false;
			}
		}
		if(ropeActive && change && GameObject.Find("Link1"))
        {
			HingeJoint2D toPull = attachedTo.AddComponent<HingeJoint2D>();
			toPull.anchor = GameObject.Find("Link1").transform.localPosition;
			toPull.connectedBody = GameObject.Find("Link1").GetComponent<Rigidbody2D>();

			DistanceJoint2D toPull1 = attachedTo.AddComponent<DistanceJoint2D>();
			toPull1.anchor = GameObject.Find("Link1").transform.localPosition;
			toPull1.connectedBody = GameObject.Find("Link1").GetComponent<Rigidbody2D>();


		    
			
			pulling = true;
			change = false;
		}
		if(pulling)
        {
			hook.GetComponent<LineRenderer>().SetPosition(1, attachedTo.transform.position);
		}
	}
}
