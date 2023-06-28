using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class throwhook : MonoBehaviour 
{
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

	void Start () 
	{
		//oldPos = transform.position;
	}

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
					curHook.GetComponent<RopeScript>().destiny = hit.point;

					if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
					{
						change = true;
					}
					ropeActive = true;
				}
				
			}
			else
			{
				//delete rope
				Destroy(curHook, 0.1f);

				Destroy(attachedTo.GetComponent<SpringJoint2D>());
				Destroy(attachedTo.GetComponent<DistanceJoint2D>());
				if (attachedTo.CompareTag("Enemy"))
				{
					EnemyManager.enemyManager.SetEnemyWeight(attachedTo, EnemyManager.enemyManager.GetEnemyWeight(attachedTo) * 2);
				}
				ropeActive = false;
				change = false;
				pulling = false;
			}
		}

		if(ropeActive && change && GameObject.Find("Link1"))
        {
			GameObject Link1 = GameObject.Find("Link1");

			DistanceJoint2D toPull = attachedTo.AddComponent<DistanceJoint2D>();
			toPull.anchor = Link1.transform.localPosition;
			toPull.connectedBody = Link1.GetComponent<Rigidbody2D>();
			toPull.autoConfigureDistance = false;
			toPull.maxDistanceOnly = true;
			toPull.distance = 0.2f;

			SpringJoint2D toPull1 = attachedTo.AddComponent<SpringJoint2D>();
			toPull1.anchor = Link1.transform.localPosition;
			toPull1.connectedBody = Link1.GetComponent<Rigidbody2D>();
			if (attachedTo.CompareTag("Enemy"))
			{
				EnemyManager.enemyManager.SetEnemyWeight(attachedTo, (int)(EnemyManager.enemyManager.GetEnemyWeight(attachedTo) * 0.5f));
			}
				

			pulling = true;
			change = false;
		}
	}
}
