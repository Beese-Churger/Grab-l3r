﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class throwhook : MonoBehaviour 
{
	[SerializeField] private InputActionReference grappleAction;

	public GameObject hook;
	[SerializeField] private GameObject pivot;
	[SerializeField] private Transform crosshair;
	[SerializeField] private SpriteRenderer crosshairSprite;
	[SerializeField] private GameObject hookSprite;
	[SerializeField] private Image cursor;
	public InputActionReference pointer;

	public bool ropeActive;

	public int maxDistance = 10;

	public LayerMask ropeLayerMask;

	public GameObject attachedTo;

	public RopeScript ropeScript;

	GameObject curHook;

	bool change = false;

	public bool pulling = false;
    private Terrain.TerrainType type;

	public HookContext hookContext;

	private float lastInputTime;
	private float inputDelay = 0.19f;
	Vector2 playerVel;
	Vector2 attachedVel;
	public List<Vector2> avgVel;
	public List<Vector2> avgAttachedVel;
	Vector3 oldPos;
	Vector3 oldAttachedPos;
	public enum HookContext
	{
		HOOK_SMALL,
		HOOK_BIG,
	};
	void Start () 
	{
		//oldPos = transform.position;
	}

    private void Awake()
    {
		hookContext = HookContext.HOOK_BIG;
		lastInputTime = Time.time;
		oldPos = transform.position;

		cursor = GameObject.Find("Cursor").GetComponent<Image>();

    }


	public void destroyHook()
    {
		Destroy(attachedTo.GetComponent<SpringJoint2D>());
		Destroy(attachedTo.GetComponent<DistanceJoint2D>());
		hookContext = HookContext.HOOK_BIG;
		if (attachedTo.GetComponent<SmallEnemy>())
		{
			attachedTo.GetComponent<SmallEnemy>().isHooked = false;
		}

		ropeActive = false;
		change = false;
		pulling = false;

		//delete rope
		Destroy(curHook, 0.1f);
	}
    void Update () 
	{		
		Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointer.action.ReadValue<Vector2>().x, pointer.action.ReadValue<Vector2>().y, 0f));
		cursor.transform.position = pointer.action.ReadValue<Vector2>();
		Vector3 facingDirection = worldMousePosition - transform.position;
		float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

		if (aimAngle < 0f)
		{
			aimAngle = Mathf.PI * 2 + aimAngle;
		}

		SetCrosshairPosition(aimAngle);

		Vector3 diff = Camera.main.ScreenToWorldPoint(pointer.action.ReadValue<Vector2>()) - pivot.transform.position;
		diff.Normalize();

		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		pivot.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

		Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

		if (GameObject.Find("LegsToExplode"))
			return;

		if (lastInputTime + inputDelay < Time.time)
		{
			if (grappleAction.action.triggered)
			{
				if (!ropeActive)
				{
					RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, maxDistance, ropeLayerMask);
					if (hit.collider != null)
					{
						if (hit.transform.TryGetComponent<Terrain>(out var terrain))
						{
							type = terrain.GetTerrainType();
							if (type != Terrain.TerrainType.concreate)
							{
								attachedTo = hit.transform.gameObject;
								AudioManager.Instance.PlaySFX("hook_shoot2", transform.position);
								curHook = Instantiate(hook, transform.position, Quaternion.identity);
								curHook.GetComponent<RopeScript>().destiny = hit.point;
								curHook.GetComponent<RopeScript>().SetCanHook(true);
								if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
								{
									change = true;
								}
								ropeActive = true;

							}
							else
							{
								AudioManager.Instance.PlaySFX("hook_shoot1", transform.position);
								curHook = Instantiate(hook, transform.position, Quaternion.identity);
								curHook.GetComponent<RopeScript>().SetCanHook(false);
								curHook.GetComponent<RopeScript>().destiny = hit.point;
							}
						}
						else
						{
							// for entities
							attachedTo = hit.transform.gameObject;
							AudioManager.Instance.PlaySFX("hook_shoot2", transform.position);
							curHook = Instantiate(hook, transform.position, Quaternion.identity);
							curHook.GetComponent<RopeScript>().destiny = hit.point;
							curHook.GetComponent<RopeScript>().SetCanHook(true);
							if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
							{
								change = true;
								if (attachedTo.GetComponent<SmallEnemy>())
								{
									hookContext = HookContext.HOOK_SMALL;
									attachedTo.GetComponent<SmallEnemy>().isHooked = true;
								}
								else
								{
									hookContext = HookContext.HOOK_BIG;
								}
							}
							ropeActive = true;
						}
						hookSprite.GetComponent<SpriteRenderer>().enabled = false;
					}
					else
					{
						curHook = Instantiate(hook, transform.position, Quaternion.identity);
						curHook.GetComponent<RopeScript>().SetCanHook(false);
						curHook.GetComponent<RopeScript>().destiny = transform.position + (aimDirection * maxDistance);
						hookSprite.GetComponent<SpriteRenderer>().enabled = false;
						ropeActive = true;
					}
				}
				else
				{
					//delete rope
					destroyHook();

					launch();
					if (attachedTo != null)
                    {
						if(attachedTo.GetComponent<SmallEnemy>())
                        {
							launchAttached();
						}
					}
					hookSprite.GetComponent<SpriteRenderer>().enabled = true;
				}
				lastInputTime = Time.time;
			}
		}



		if (ropeActive && change && GameObject.Find("Link1"))
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

			pulling = true;
			change = false;
		}

		gameObject.GetComponent<SimpleController>().SetHook(ropeActive);

		
		if(avgVel.Count < 5)
			avgVel.Add((transform.position - oldPos));
		else
        {
			avgVel.RemoveAt(0);
			avgVel.Add((transform.position - oldPos));
		}
		oldPos = transform.position;
		if(attachedTo)
        {
			if (avgAttachedVel.Count < 5)
				avgAttachedVel.Add((attachedTo.transform.position - oldAttachedPos));
			else
			{
				avgAttachedVel.RemoveAt(0);
				avgAttachedVel.Add((attachedTo.transform.position - oldAttachedPos));
			}
			

			oldAttachedPos = attachedTo.transform.position;
		}
			
	}

	// Move the aiming crosshair based on aim angle
	private void SetCrosshairPosition(float aimAngle)
	{
		if (!crosshairSprite.enabled)
		{
			crosshairSprite.enabled = true;
		}

		float x = transform.position.x + 1f * Mathf.Cos(aimAngle);
		float y = transform.position.y + 1f * Mathf.Sin(aimAngle);

		Vector3 crossHairPosition = new Vector3(x, y, 0);
		crosshair.transform.position = crossHairPosition;
	}
	public void launch()
    {
		Vector2 vel = new Vector2(0, 0);
		for (int i = 0; i < avgVel.Count; ++i)
		{
			vel += avgVel[i];
		}
		playerVel = vel / (5 * Time.deltaTime);
		gameObject.GetComponent<Rigidbody2D>().velocity = playerVel;
    }

	public void launchAttached()
    {
		Vector2 attachvel = new Vector2(0, 0);
		for (int i = 0; i < avgAttachedVel.Count; ++i)
		{
			attachvel += avgAttachedVel[i];
		}
		attachedVel = attachedVel / (5 * Time.deltaTime);
		attachedTo.GetComponent<Rigidbody2D>().velocity = attachvel; 
	}
}

