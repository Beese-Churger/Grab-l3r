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

		if (grappleAction.action.triggered) {


			if (ropeActive == false) {

				RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, maxDistance, ropeLayerMask);
				if (hit.collider != null)
				{
					if (hit.transform.TryGetComponent<Terrain>(out var terrain))
					{
						type = terrain.GetTerrainType();
						if (type != Terrain.TerrainType.concreate)
						{
							attachedTo = hit.transform.gameObject;
							AudioManager.Instance.PlaySFX("hook_attach");
							curHook = Instantiate(hook, transform.position, Quaternion.identity);
							curHook.GetComponent<RopeScript>().destiny = hit.point;
							curHook.GetComponent<RopeScript>().SetCanHook(true);
							if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
							{
								change = true;
								//if(attachedTo.GetComponent<SmallEnemy>())
        //                        {

								//	hookContext = HookContext.HOOK_SMALL;
								//	Debug.Log("real?????????????????????????????????");
        //                        }
								//else
        //                        {
								//	hookContext = HookContext.HOOK_BIG;
        //                        }
							}
							ropeActive = true;
						}
					}
					else
					{
						// for entities
                        attachedTo = hit.transform.gameObject;
                        AudioManager.Instance.PlaySFX("hook_attach");
                        curHook = Instantiate(hook, transform.position, Quaternion.identity);
                        curHook.GetComponent<RopeScript>().destiny = hit.point;
						curHook.GetComponent<RopeScript>().SetCanHook(true);
						if (attachedTo.transform.GetComponent<Rigidbody2D>() != null)
						{
							change = true;
							if (attachedTo.GetComponent<SmallEnemy>())
							{

								hookContext = HookContext.HOOK_SMALL;
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
				Destroy(curHook, 0.1f);

				Destroy(attachedTo.GetComponent<SpringJoint2D>());
				Destroy(attachedTo.GetComponent<DistanceJoint2D>());
				hookContext = HookContext.HOOK_BIG;
				//if (attachedTo.CompareTag("Enemy"))
				//{
				//	int enemyType = EnemyManager.enemyManager.GetEnemyType(attachedTo);
				//	if (enemyType == 0)
				//	{
				//		attachedTo.GetComponent<SmallEnemy>().SetWeight(attachedTo.GetComponent<SmallEnemy>().GetWeight() * 5);
				//		attachedTo.GetComponent<SmallEnemy>().isHooked = false;
				//	}
				//	else if (enemyType == 1)
				//	{
				//		attachedTo.GetComponent<Rigidbody2D>().isKinematic = false;
				//	}

				//}
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
			//if (attachedTo.CompareTag("Enemy"))
			//{
			//	int enemyType = EnemyManager.enemyManager.GetEnemyType(attachedTo);
			//	if (enemyType == 0)
			//	{
			//		attachedTo.GetComponent<SmallEnemy>().SetWeight((int)(attachedTo.GetComponent<SmallEnemy>().GetWeight() * 0.2));
			//		attachedTo.GetComponent<SmallEnemy>().isHooked = true;
			//	}
			//	else if (enemyType == 1)
			//	{
			//		attachedTo.GetComponent<Rigidbody2D>().isKinematic = true;
			//	}
			//}
				

			pulling = true;
			change = false;
		}

		gameObject.GetComponent<SimpleController>().SetHook(ropeActive);
	}
}

