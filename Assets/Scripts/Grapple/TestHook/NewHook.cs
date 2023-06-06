using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHook : MonoBehaviour
{
	enum HookState
	{
		HOOK_RETRACTED = -1,
		HOOK_IDLE = 0,
		HOOK_RETRACT_START = 1,
		HOOK_RETRACT_END = 3,
		HOOK_FLYING,
		HOOK_GRABBED,
		HOOK_GRABBED_NOHOOK,
		HOOK_ATTACH_GROUND,
		HOOK_ATTACH_ENTITY,
	};
	public Vector2 m_HookPos = new Vector2(0, 0);
	public Vector2 m_HookDir = new Vector2(0, 0);
	public float m_HookFireSpeed = 100f;
	public float m_HookLength = 10f;
	private Vector2 m_HookBase = new Vector2(0, 0); 
	private bool m_NewHook = false;
	private bool m_Reset = true;
	public GameObject HookAnchor;
	public GameObject GrappleGun;
	public GameObject GrapplePivot;
	public LayerMask ropeLayerMask;
	public Rigidbody2D rBody;
	public float m_HookDistance = 20f;
	public float m_MaxHookDistance = 40f;
	public float m_HookDragAccel = 3f;
	public float m_HookDragSpeed = 15f;
	public LineRenderer m_lineRenderer;
	private float horizontalInput;

	HookState m_HookState = HookState.HOOK_IDLE;
	// Start is called before the first frame update
	void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
	}

    private void Update()
    {
		horizontalInput = Input.GetAxis("Horizontal");
	}
    private void FixedUpdate()
    {
		Vector2 m_PivotPos = GrapplePivot.transform.position;

		Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
		Vector3 facingDirection = worldMousePosition - transform.position;
		float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
		if (aimAngle < 0f)
		{
			aimAngle = Mathf.PI * 2 + aimAngle;
		}

		Vector2 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

		m_HookDir = aimDirection;

		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (m_HookState == HookState.HOOK_IDLE)
			{
				m_HookState = HookState.HOOK_FLYING;
				m_HookPos = m_PivotPos + aimDirection * 1.5f;
				m_HookDir = aimDirection;
			}
		}
        else
        {
            m_HookState = HookState.HOOK_IDLE;
            m_HookPos = m_PivotPos;
        }
        // do hook
        if (m_HookState == HookState.HOOK_IDLE)
		{
			//SetHookedPlayer(-1);
			m_HookPos = m_PivotPos;
		}
		else if (m_HookState >= HookState.HOOK_RETRACT_START && m_HookState < HookState.HOOK_RETRACT_END)
		{
			m_HookState++;
		}
		else if (m_HookState == HookState.HOOK_RETRACT_END)
		{
			m_HookState = HookState.HOOK_RETRACTED;
		}
		else if (m_HookState == HookState.HOOK_FLYING)
		{
			Vector2 NewPos = m_HookPos + m_HookDir * m_HookFireSpeed;

			if ((!m_NewHook && Vector2.Distance(m_PivotPos, NewPos) > m_HookLength) || (m_NewHook && Vector2.Distance(m_HookBase, NewPos) > m_HookLength))
			{
				m_HookState = HookState.HOOK_RETRACT_START;
				NewPos = m_PivotPos + (NewPos - m_PivotPos).normalized * m_HookLength;
				m_Reset = true;
			}

			// make sure that the hook doesn't go though the ground
			bool GoingToHitGround = false;
			bool GoingToRetract = false;
			bool GoingThroughTile = false;
			int teleNr = 0;

			RaycastHit2D hit = Physics2D.Raycast(m_HookPos, NewPos, ropeLayerMask);
			
			// m_NewHook = false;

			if (hit.collider != null)
			{
				Debug.Log(hit.collider.gameObject);
				if (hit.collider.tag == "TILE_NOHOOK")
					GoingToRetract = true;
				else if (hit.collider.tag == "TILE_HOOKPASS")
					GoingThroughTile = true;
				else
					GoingToHitGround = true;
				m_Reset = true;
			}

		

			if (m_HookState == HookState.HOOK_FLYING)
			{
				// check against ground
				if (GoingToHitGround)
				{
					m_HookState = HookState.HOOK_GRABBED;
				}
				else if (GoingToRetract)
				{
					m_HookState = HookState.HOOK_RETRACT_START;
				}

				//if (GoingThroughTele && m_pWorld && m_pTeleOuts && !m_pTeleOuts->empty() && !(*m_pTeleOuts)[teleNr - 1].empty())
				//{
				//	m_TriggeredEvents = 0;
				//	SetHookedPlayer(-1);

				//	m_NewHook = true;
				//	int RandomOut = m_pWorld->RandomOr0((*m_pTeleOuts)[teleNr - 1].size());
				//	m_HookPos = (*m_pTeleOuts)[teleNr - 1][RandomOut] + TargetDirection * PhysicalSize() * 1.5f;
				//	m_HookDir = TargetDirection;
				//	m_HookTeleBase = m_HookPos;
				//}
				//else
				//{
					m_HookPos = NewPos;
				DrawRopeNoWaves(NewPos);
				//}
			}
		}

		if (m_HookState == HookState.HOOK_GRABBED)
		{ 

			if (Vector2.Distance(m_HookPos, m_PivotPos) > m_MaxHookDistance)
			{
				Vector2 HookVel = (m_HookPos - m_PivotPos).normalized * m_HookDragAccel;
				// the hook as more power to drag you up then down.
				// this makes it easier to get on top of an platform
				if (HookVel.y > 0)
					HookVel.y *= 0.3f;

				// the hook will boost it's power if the player wants to move
				// in that direction. otherwise it will dampen everything abit
				if ((HookVel.x < 0 && horizontalInput < 0) || (HookVel.x > 0 && horizontalInput > 0))
					HookVel.x *= 0.95f;
				else
					HookVel.x *= 0.75f;

				Vector2 NewVel = rBody.velocity + HookVel;

				// check if we are under the legal limit for the hook
				if (NewVel.magnitude < m_HookDragSpeed || NewVel.magnitude < rBody.velocity.magnitude)
					rBody.velocity = NewVel; // no problem. apply
			}

			if (m_HookState == HookState.HOOK_GRABBED)
			{
				//SetHookedPlayer(-1);
				m_HookState = HookState.HOOK_RETRACTED;
				m_HookPos = m_PivotPos;
			}
			Debug.Log(m_HookState);
		}

	}

	void DrawRopeNoWaves(Vector2 NewPos)
	{
		m_lineRenderer.SetPosition(0, GrapplePivot.transform.position);
		m_lineRenderer.SetPosition(1, NewPos);
	}
}
