using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Terrain;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public GameManager manager;
    public bool isActive;

    public enum ObstacleType
    {
        water,
        spikes,
        electricity
    }
    [SerializeField] ObstacleType obstacleType;

    // activate levels obstacles
    private void Awake()
    {
        isActive = true;
    }

    // check collision if object is active
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            if (collision.transform.CompareTag("Player"))
            {
                manager.TakeDamage();
                Debug.Log("Obstacle collision");
            }
        }
    }

    // call function to disable obstacle on pressure plate press
    public void DisableObstacle()
    {
        isActive = false;
        Debug.Log(isActive);
    }

    private void Update()
    {
        if (!isActive)
        {
            // add animation or other function to different types 
            // destroy obstacle?
            switch (obstacleType)
            {
                case ObstacleType.water:
                    break;
                case ObstacleType.spikes:
                    break;
                case ObstacleType.electricity:
                    break;
            }
        }
    }
}
