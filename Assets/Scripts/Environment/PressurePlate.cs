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
    public bool destroyBoss;
    public Animator animator;

    private Obstacle.ObstacleType type;
    private bool isObstacle;
    private bool isDoor;
    private bool elecActive = true;

    public bool bossDeactivatedElectricity;
    public bool unlockGrinder;

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
                // Increase Boss Phase
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Boss.instance.SetPPlate(true);              
                }
                //  Opens Door
                if (isDoor && !isDoorOpen)
                {
                    if (obstacle != null)
                    {
                        obstacle.OpenDoor();
                    }
                }
                // Closes Door
                else if (isDoorOpen)
                {
                    if (obstacle != null)
                    {
                        obstacle.CloseDoor();
                    }

                }
                // Activate Moving Platforms
                else if (!isObstacle && !isDoor)
                {
                    if (terrain != null)
                    {
                        terrain.ActivateMovingPlatform();
                    }
                }
                // Deactivates electricity
                if (isElectricity && elecActive)
                {
                   foreach (Obstacle obj in electricityControlled)
                   {
                        obj.DeactivateElectricity();
                   }
                    elecActive = false;
                }
                // Deactivates Boss Grinder Attack
                if (bossDeactivatedElectricity)
                {
                    Boss.instance.SetElectric(false);
                    Boss.instance.AddAttack(Boss.ATTACK.CRUSH);
                }
                if (unlockGrinder)
                {
                    Boss.instance.AddAttack(Boss.ATTACK.GRINDER);
                }

                // Destroy the boss
                if (destroyBoss && Boss.instance.gameObject.activeInHierarchy)
                {
                    GameObject.Find("BossToExplode").GetComponent<ExplodeOnAwake>().explode("TheCollector");
                    Boss.instance.gameObject.SetActive(false);
                }
            }
        } 
    }

    // activate obstacles, open/close doors, deactivate mnoving platforms
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != "FOV")
        {
            objectsInTrigger.Remove(collision.gameObject);
            if (objectsInTrigger.Count <= 0)
            {
                animator.SetBool("isPressed", false);
                // Decreases Boss Phase
                if (GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
                {
                    Boss.instance.SetPPlate(false);
                }
                // Activates all electricity in control
                if (isElectricity && !elecActive)
                {
                    foreach (Obstacle obj in electricityControlled)
                    {
                        obj.ActivateElectricity();
                    }
                    elecActive = true;
                }
                // Activate all controlled electricity
                if (bossDeactivatedElectricity)
                {
                    Boss.instance.SetElectric(true);
                    Boss.instance.RemoveAttack(Boss.ATTACK.CRUSH);
                }
                if (unlockGrinder)
                {
                    Boss.instance.RemoveAttack(Boss.ATTACK.GRINDER);
                }
                // Close the door
                if (isDoor && !isDoorOpen)
                {
                    obstacle.CloseDoor();
                }
                // Open the door
                else if (isDoorOpen)
                {
                    if (obstacle != null)
                    {
                        obstacle.OpenDoor();
                    }
                }
                // Deactivate Moving Platform
                else if (!isObstacle && !isDoor)
                {
                    terrain.DeactivateMovingPlatform();
                }
                // Disable obstacle
                else if (isObstacle)
                {
                    obstacle.ActivateObstacle();
                }
            }
        }
    }
}
