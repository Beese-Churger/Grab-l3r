using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public bool isGrabbing = false;
    public float speed = 1f;
    public ParticleSystem dust;

    private Rigidbody2D rb;
    public static Vector2 checkpointPos;
    private Vector2 movementDir;
    private Vector2 playerPos;

    private void Awake()
    {
        playerPos = transform.position;
        Debug.Log("player" + playerPos);
        Debug.Log("checkpoint" + checkpointPos);
    }

    void Start()
    {
       
        if (GameManager.instance.resetPlayer == true)
        {
            transform.position = checkpointPos;
            GameManager.instance.resetPlayer = false;
            Debug.Log("Trans1" + transform.position);
        }
        else
        {
            transform.position = playerPos;
            Debug.Log("Trans2" + transform.position);
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
            if (!dust.isPlaying)
            {
                dust.Play();
            }
        }
    }
}

