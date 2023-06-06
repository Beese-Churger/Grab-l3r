using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Terrain;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerObj = null;
    private GameManager gameManager = null;
    public enum ObstacleType
    {
        water,
        spikes,
        electricity
    }
    [SerializeField] ObstacleType obstacleType;

    private void Start()
    {
        gameManager = gameManager.GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            switch (obstacleType)
            {
                case ObstacleType.water:
                    break;
                case ObstacleType.spikes:
                    break;
                case ObstacleType.electricity:
                    break;
            }
            gameManager.TakeDamage();
            Debug.Log("Obstacle collision"+obstacleType);
        }
    }

}
