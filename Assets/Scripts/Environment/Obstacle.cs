using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private GameObject gameManager;
    public enum ObstacleType
    {
        water,
        spikes,
        electricity
    }

    private void Update()
    {

    }

    private void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {

        }
    }

}
