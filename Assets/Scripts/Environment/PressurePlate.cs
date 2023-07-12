using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Terrain terrain;
    public Obstacle obstacle;
    public bool isDoorOpen;
    public Animator animator;
    public List<GameObject> objectsInTrigger;

    private Obstacle.ObstacleType type;
    private bool isObstacle;
    private bool isDoor;
    private bool stepped = false;


    private void Start()
    {
        if (obstacle != null)
        {
            type = obstacle.GetObstacleType();
            if (type == Obstacle.ObstacleType.door)
            {
                isDoor = true;
                if (isDoorOpen)
                {
                    obstacle.OpenDoor();
                }
            }
            else
            {
                isDoor = false;
                isObstacle = true;
            }
        }
        else
        {
            isObstacle = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.position.y < collision.transform.position.y)
        {
            if (isObstacle)
            {
                if (obstacle != null)
                    obstacle.DisableObstacle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (!stepped)
        {
            //stepped = true;
            if (collision.gameObject.name != "FOV")
            {
                objectsInTrigger.Add(collision.gameObject);
                animator.SetBool("isPressed", true);
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Boss.instance.SetPPlate(true);
                    return;
                }

                if (isDoor && !isDoorOpen)
                {
                    if (obstacle != null)
                    {
                        obstacle.OpenDoor();
                    }
                }
                else if (isDoorOpen)
                {
                    if (obstacle != null)
                    {
                        obstacle.CloseDoor();
                    }

                }
                else if (!isObstacle && !isDoor)
                {
                    if (terrain != null)
                    {
                        terrain.ActivateMovingPlatform();
                    }

                }

            }
            
        }
    }

    // set move plate back up when player exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInTrigger.Remove(collision.gameObject);       
        if (objectsInTrigger.Count <= 0)
        {
            animator.SetBool("isPressed", false);
            if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
            {
                Boss.instance.SetPPlate(false);
            }
            if (isDoor && !isDoorOpen)
            {
                obstacle.CloseDoor();
            }
            else if (isDoorOpen)
            {
                if (obstacle != null)
                {
                    obstacle.OpenDoor();
                }

            }
            else if (!isObstacle && !isDoor)
            {
                terrain.DeactivateMovingPlatform();
            }
            else if (isObstacle)
            {
                obstacle.ActivateObstacle();
            }
        }
        
    }
}
