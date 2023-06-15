using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Terrain;

public class Obstacle : MonoBehaviour
{
    [SerializeField] public GameManager manager;
    public Sprite newSprite;
    public Vector2 endPos;

    private Vector2 startPos;
    private SpriteRenderer spriteRender;
    private Sprite mySprite;
    private bool isActive;
    private bool isReactivating;


    public enum ObstacleType
    {
        spikes,
        electricity,
        door
    }
    [SerializeField] ObstacleType obstacleType;

    // activate obstacles
    private void Awake()
    {
        isActive = true;
    }

    private void Start()
    {
        startPos = transform.position;
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        mySprite = spriteRender.sprite;
    }

    // check collision if object is active
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            if (collision.transform.CompareTag("Player"))
            {
                // Damage player
                manager.TakeDamage();
                Debug.Log("Obstacle collision");
            }
        }
    }

    // call function to disable obstacle on pressure plate press
    public void DisableObstacle()
    {
        isActive = false;
    }

    // reactivate obstacle
    public void ActivateObstacle()
    {
        isActive = true;
    }

    private void Update()
    {
        if (!isActive)
        {
            // TODO: add animation or other function to different types 
            // destroy obstacle?
            switch (obstacleType)
            {
                case ObstacleType.spikes:
                    if (transform.position.y > endPos.y)
                    {
                        transform.Translate(0, -0.01f, 0);
                    }
                    break;
                case ObstacleType.electricity:
                    spriteRender.sprite = newSprite;
                    break;
            }
        }
        else
        {
            if (isReactivating)
            {
                switch (obstacleType)
                {
                    case ObstacleType.spikes:
                        if (transform.position.y < startPos.y)
                        {
                            transform.Translate(0, 0.01f, 0);
                        }
                        break;
                    case ObstacleType.electricity:
                        spriteRender.sprite = mySprite;
                        break;
                }
            }
        }
    }
}
