using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public static Vector2 checkpointPos = new Vector2(-2,0);
    public float speed = 1f;

    private Rigidbody2D rb;
    private Vector2 movementDir;

    private Vector2 playerPos = new Vector2(0, 0);

    enum HookState
    {
        HOOK_RETRACTED,
        HOOK_IDLE,
        HOOK_RETRACT_START,
        HOOK_RETRACT_END,
        HOOK_FLYING,
        HOOK_GRABBED,
        HOOK_GRABBED_NOHOOK,
        HOOK_ATTACH_GROUND,
        HOOK_ATTACH_ENTITY,
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // set player position to last checkpoint on respawn
        GameObject.FindGameObjectWithTag("Player").transform.position = checkpointPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Player movement for testing purpouses
    void Update()
    {
        movementDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate(speed * movementDir * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check collision with terrain
        if (collision.transform.tag == "Terrain")
        {
            // TODO: check terrain surface type
            Debug.Log("Collision with terrain");
        }
    }
}
