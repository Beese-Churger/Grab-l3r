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
    private Vector2 playerPos;

    private void Awake()
    {
        playerPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //var point = GameManager.instance.GetCheckPointPos();
        //transform.position = point;
        if (GameManager.instance.resetPlayer == true)
        {
            transform.position = playerPos;
            GameManager.instance.resetPlayer = false;
        }
        else
        {
            transform.position = checkpointPos;
        }
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
        GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal") * 5, GetComponent<Rigidbody2D>().velocity.y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300f);
        }
    }
}

