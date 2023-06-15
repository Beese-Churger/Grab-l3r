using UnityEngine;
using UnityEngine.Pool;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] public Terrain terrain;
    [SerializeField] public Obstacle obstacle;
    
    private bool back = false;
    private Vector2 startPos;

    public Vector2 endPos;
    public bool isObstacle;
    public bool isDoor;

    // set start position as game objects position in editor
    private void Start()
    {
        startPos = transform.position;
    }

    // press plate if player stays on trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.position.y < collision.transform.position.y)
        {
            // if isObstacle is unchecked, pressureplate activates moving platform
            if (transform.position.y > endPos.y)
            {
                transform.Translate(0, -0.01f, 0);
                back = false;
            }

            // if pressureplate is set to isObstacle, pressureplate disaples obtsacles functionality
            // when pressureplate is pushed. Other wise activates moving platform
            if (isObstacle)
            {
                obstacle.DisableObstacle();
            }
            else if (isDoor)
            {
                obstacle.OpenDoor();
            }
            else
            {
                terrain.ActivateMovingPlatform();
            }
        }
    }
    
    // set move plate back up when player exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        back = true;
    }

    private void Update()
    {
        // if player has exited the plate, plate comes back up
        if (back)
        {
            if (transform.position.y < startPos.y)
            {
                transform.Translate(0, 0.01f, 0);
            }
            else
            {
                back = false;
            }
        }

        // reactivate obstacle when plate not pressed
        if(isObstacle && transform.position.y >= startPos.y && !obstacle.isReactivating)
        {
            obstacle.ActivateObstacle();
        }
    }
}
