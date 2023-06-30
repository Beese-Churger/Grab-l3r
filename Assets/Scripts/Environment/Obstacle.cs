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
        door,
        water
    }
    [SerializeField] ObstacleType obstacleType;

    public Sprite newSprite;
    //public Vector2 endPos;
    public Animator animator;

    //private Vector2 startPos;
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
        //startPos = transform.position;
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        mySprite = spriteRender.sprite;
        myCollider = gameObject.GetComponent<Collider2D>();
    }

    // check collision if object is active
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            if (collision.transform.CompareTag("Player") & obstacleType !=ObstacleType.door)
            {
                // Damage player
                GameManager.instance.TakeDamage();
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
        animator.SetBool("isOpen", true);
        if (myCollider != null)
            myCollider.enabled = false;
    }

    public void CloseDoor()
    {
        animator.SetBool("isOpen", false);
        if (myCollider != null)
            myCollider.enabled = true;
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
