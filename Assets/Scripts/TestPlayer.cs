using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public bool isGrabbing = false;
    public float speed = 1f;

    private Rigidbody2D rb;
    public static Vector2 checkpointPos;
    private Vector2 movementDir;
    private Vector2 playerPos = new Vector2(0, 0);

    private void Awake()
    {
        // set player position to last checkpoint on respawn
        transform.position = playerPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //var point = GameManager.instance.GetCheckPointPos();
        //transform.position = point;
        transform.position = checkpointPos;
    }

    public void SetCheckPoint(Vector2 point)
    {
        checkpointPos = point;
    }

    public Vector2 GetCheckPointPos()
    {
        return checkpointPos;
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

