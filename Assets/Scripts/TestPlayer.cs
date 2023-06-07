using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public static Vector2 checkpointPos;
    public bool isGrabbing = false;
    public float speed = 1f;

    private Rigidbody2D rb;
    private Vector2 movementDir;

    private Vector2 playerPos = new Vector2(0, 0);
    private void Awake()
    {
        // set player position to last checkpoint on respawn
        checkpointPos = transform.position;
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
        //movementDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //transform.Translate(speed * movementDir * Time.deltaTime);

        GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal") * 5, GetComponent<Rigidbody2D>().velocity.y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check collision with terrain
        if (collision.transform.CompareTag("Terrain"))
        {
            // TODO: check terrain surface type
            isGrabbing = true;
            //Debug.Log("Collision with terrain");
        }
    }
}

