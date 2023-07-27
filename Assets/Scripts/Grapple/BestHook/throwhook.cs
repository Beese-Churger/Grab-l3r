using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class throwhook : MonoBehaviour 
{
	[SerializeField] private InputActionReference grappleAction;

	public GameObject hook;

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
	Vector3 playerVel;

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
		Vector3 facingDirection = worldMousePosition - transform.position;
		float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

		if (aimAngle < 0f)
		{
			aimAngle = Mathf.PI * 2 + aimAngle;
		}

		Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

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
					}
					else
					{
						curHook = Instantiate(hook, transform.position, Quaternion.identity);
						curHook.GetComponent<RopeScript>().SetCanHook(false);
						curHook.GetComponent<RopeScript>().destiny = transform.position + (aimDirection * maxDistance);
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
							launchAttached();
					}
				}
				lastInputTime = Time.time;
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

			pulling = true;
			change = false;
		}

		gameObject.GetComponent<SimpleController>().SetHook(ropeActive);
		oldPos = transform.position;
		if(attachedTo)
			oldAttachedPos = attachedTo.transform.position;
	}

	public void launch()
    {
		gameObject.GetComponent<Rigidbody2D>().velocity = (transform.position - oldPos )/ Time.deltaTime;
    }

	public void launchAttached()
    {
		attachedTo.GetComponent<Rigidbody2D>().velocity = (attachedTo.transform.position - oldAttachedPos) / Time.deltaTime;
	}
}

