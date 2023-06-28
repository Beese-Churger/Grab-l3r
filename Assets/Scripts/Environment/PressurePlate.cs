using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Terrain terrain;
    public Obstacle obstacle;
    //public Vector2 endPos;
    public bool isObstacle;
    public bool isDoor;
    public Animator animator;

    private bool back = false;
    public List<GameObject> objectsInTrigger;
    //private Vector2 startPos;

    // set start position as game objects position in editor
    private void Start()
    {
        //startPos = transform.position;
    }

    // press plate if player stays on trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.position.y < collision.transform.position.y)
        {
            // if isObstacle is unchecked, pressureplate activates moving platform
            //if (transform.position.y > endPos.y)
            //{
            //    transform.Translate(0, -0.01f, 0);
            //    back = false;
            //}

            // if pressureplate is set to isObstacle, pressureplate disaples obtsacles functionality
            // when pressureplate is pushed. Other wise activates moving platform
            if (isObstacle)
            {
                obstacle.DisableObstacle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInTrigger.Add(collision.gameObject);
        animator.SetBool("isPressed", true);
        if (isDoor)
        {
            obstacle.OpenDoor();
        }
        else if (!isObstacle && !isDoor)
        {
            terrain.ActivateMovingPlatform();
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

    private void Update()
    {
        // if player has exited the plate, plate comes back up
        //if (back)
        //{
        //    if (transform.position.y < startPos.y)
        //    {
        //        transform.Translate(0, 0.01f, 0);
        //    }
        //    else
        //    {
        //        back = false;
        //    }
        //}

        // reactivate obstacle when plate not pressed
        //if(isObstacle && transform.position.y >= startPos.y)
        //{
        //    obstacle.ActivateObstacle();
        //}
    }
}
