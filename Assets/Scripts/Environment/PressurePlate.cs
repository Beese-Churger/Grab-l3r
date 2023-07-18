using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Obstacle[] electricityControlled;
    public Terrain terrain;
    public Obstacle obstacle;
    public bool isDoorOpen;
    public Animator animator;
    public List<GameObject> objectsInTrigger;

    private Obstacle.ObstacleType type;
    private bool isObstacle;
    private bool isDoor;
    public bool isElectricity = false;
    private bool elecActive = true;

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
                    if (!stepped)
                    {
                        Boss.instance.SetPPlate(true);
                        stepped = true;
                        return;
                    }
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
                if (stepped)
                {
                    Boss.instance.SetPPlate(false);
                    stepped = false;
                }
            }
            if (isElectricity && !elecActive)
            {
                foreach (Obstacle obj in electricityControlled)
                {
                    obj.ActivateElectricity();
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
