using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] public Terrain terrain;
    [SerializeField] public Obstacle obstacle;
    private Vector2 startPos;
    public Vector2 endPos;
    public bool isObstacle;
    private bool back = false;

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
           if(transform.position.y<collision.transform.position.y)
           {
                transform.Translate(0, -0.01f, 0);
                back = false;

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

    // detach player from plate transform
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("Enemy"))
        {
            back = true;
        }
    }

    private void Update()
    {
        if (back)
        {
            if(transform.position.y < startPos.y) 
            {
                transform.Translate(0, 0.01f, 0);
            }
            else
            {
                back = false;
            }
        }
    }
}
