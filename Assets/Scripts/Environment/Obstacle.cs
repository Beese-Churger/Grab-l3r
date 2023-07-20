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
    public Animator animator;

    private SpriteRenderer spriteRender;
    private Collider2D myCollider;
    private Sprite mySprite;
    private bool isActive;

    private void Awake()
    {
        isActive = true;
    }

    private void Start()
    {
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        mySprite = spriteRender.sprite;
        myCollider = gameObject.GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            if (collision.transform.CompareTag("Player") & obstacleType !=ObstacleType.door)
            {
                // For the hole
                if (obstacleType == ObstacleType.water)
                {
                    GameManager.instance.InstantDeath();
                }
                else
                {
                    GameManager.instance.TakeDamage();
                }
            }
        }
    }

    public void DisableObstacle()
    {
        isActive = false;
    }

    public void ActivateObstacle()
    {
        isActive = true;
    }

    public void OpenDoor()
    {
        animator.SetBool("isOpen", true);
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
        AudioManager.Instance.PlaySFX("door_open");
    }

    public void CloseDoor()
    {
        animator.SetBool("isOpen", false);
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }
        AudioManager.Instance.PlaySFX("door_close");
    }
    public void DeactivateElectricity()
    {
        animator.SetBool("Active", false);
        //spriteRender.enabled = false;
        if (myCollider != null)
            myCollider.enabled = false;
    }
    public void ActivateElectricity()
    {
        animator.SetBool("Active", true);
        //spriteRender.enabled = true;
        myCollider.enabled = true;
    }
    public ObstacleType GetObstacleType()
    {
        return obstacleType;
    }

    private void Update()
    {
        if (!isActive)
        {
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
