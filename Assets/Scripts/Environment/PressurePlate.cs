using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] public Terrain terrain;
    private Vector2 startPos;
    public Vector2 endPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // TODO:different activations when plate is pressed
            if(transform.position.y > endPos.y)
            {
                transform.Translate(0, -0.01f, 0);
            }
            terrain.ActivateMovingPlatform();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
