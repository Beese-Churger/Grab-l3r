using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] public Terrain terrain;
    [SerializeField] public Obstacle obstacle;
    private Vector2 startPos;
    public Vector2 endPos;
    public bool isObstacle;

    // set start position as game objects position in editor
    private void Start()
    {
        startPos = transform.position;
    }

    // check if player is on the pressureplate
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // if pressureplate is set to isObstacle, pressureplate disaples obtsacles functionality
            // when pressureplate is pushed
         
                // if isObstacle is unchecked, pressureplate activates moving platform
            if (transform.position.y > endPos.y)
            {
                transform.Translate(0, -0.01f, 0);

                if (isObstacle)
                {
                    obstacle.DisableObstacle();
                }
                else
                {
                    terrain.ActivateMovingPlatform();
                }
            }
        }
    }

    // set player transform to be the same as pressureplates to avoid twitching
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    // detach player from plate transform
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
