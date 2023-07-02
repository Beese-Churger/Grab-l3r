using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Terrain terrain;
    public Obstacle obstacle;
    public bool isObstacle;
    public bool isDoor;
    public bool isOppDoor;
    public Animator animator;

    private bool back = false;
    public List<GameObject> objectsInTrigger;

    private void Start()
    {
        if(isOppDoor)
        {
            obstacle.OpenDoor();
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
        if (collision.gameObject.name != "FOV")
        {
            objectsInTrigger.Add(collision.gameObject);
            animator.SetBool("isPressed", true);
            if (isDoor)
            {
                if (obstacle != null)
                    obstacle.OpenDoor();
            }
            if(isOppDoor)
            {
                if (obstacle != null)
                    obstacle.CloseDoor();
            }
            else if (!isObstacle && !isDoor)
            {
                if (terrain != null)
                    terrain.ActivateMovingPlatform();
            }
            else
            {
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Debug.Log("Boss Stepped Blah");
                    Boss.instance.SetPPlate(true);
                }
            }
        }
    }

    // set move plate back up when player exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInTrigger.Remove(collision.gameObject) ;

        if(objectsInTrigger.Count <= 0)
        {
            animator.SetBool("isPressed", false);
            back = true;
            if (isDoor)
            {
                obstacle.CloseDoor();
            }
            if(isOppDoor)
            {
                if (obstacle != null)
                    obstacle.OpenDoor();
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
