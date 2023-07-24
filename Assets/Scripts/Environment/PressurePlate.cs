using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Obstacle[] electricityControlled;
    public List<GameObject> objectsInTrigger;
    public Terrain terrain;
    public Obstacle obstacle;
    public bool isDoorOpen;
    public bool isElectricity;
    public Animator animator;

    private Obstacle.ObstacleType type;
    private bool isObstacle;
    private bool isDoor;
    private bool elecActive = true;

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
            else if (type == Obstacle.ObstacleType.electricity)
            {
                isElectricity = true;
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

    // dissable obstacle that is affected by the pressure plate
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

    // open/close door, activate moving platform
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "FOV")
        {
            objectsInTrigger.Add(collision.gameObject);
            animator.SetBool("isPressed", true);
            if (objectsInTrigger.Count < 2)
            {
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Boss.instance.SetPPlate(true);              
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

                if (isElectricity && elecActive)
                {
                   foreach (Obstacle obj in electricityControlled)
                   {
                        obj.DeactivateElectricity();
                   }
                    if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                    {
                        Boss.instance.SetElectric(false);
                    }
                    elecActive = false;
                }
            }
        } 
    }

    // activate obstacles, open/close doors, deactivate mnoving platforms
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

            if (isElectricity && !elecActive)
            {
                foreach (Obstacle obj in electricityControlled)
                {
                    obj.ActivateElectricity();
                }
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Boss.instance.SetElectric(true);
                }
                elecActive = true;
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
