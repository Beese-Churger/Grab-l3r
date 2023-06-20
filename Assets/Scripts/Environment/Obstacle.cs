using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Terrain;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        spikes,
        electricity,
        door
    }
    [SerializeField] ObstacleType obstacleType;

    [SerializeField] public GameManager manager;
    public Sprite newSprite;
    public Vector2 endPos;

    private Vector2 startPos;
    private SpriteRenderer spriteRender;
    private Sprite mySprite;
    private bool isActive;
    private Collider2D myCollider;

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
        myCollider = gameObject.GetComponent<Collider2D>();
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

    public void OpenDoor()
    {
        myCollider.enabled = false;
        Debug.Log("Collider.enabled" + myCollider.enabled);
        spriteRender.color = Color.red;
    }

    public void CloseDoor()
    {
        myCollider.enabled = true;
        Debug.Log("Collider.enabled" + myCollider.enabled);
        spriteRender.color = Color.white;
    }

    private void Update()
    {
        if (!isActive)
        {
            // TODO: add animation or other function to different types 
            // destroy obstacle?
            switch (obstacleType)
            {
                case ObstacleType.electricity:
                    spriteRender.sprite = newSprite;
                    break;
            }
        }
        else
        {
                switch (obstacleType)
                {
                    case ObstacleType.electricity:
                        spriteRender.sprite = mySprite;
                        break;
                }
        }
    }
}
