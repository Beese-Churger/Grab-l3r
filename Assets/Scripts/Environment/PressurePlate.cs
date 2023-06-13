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

    private void OnTriggerStay2D(Collider2D collision)
    {
        // if pressureplate is set to isObstacle, pressureplate disaples obtsacles functionality
        // when pressureplate is pushed
        if (transform.position.y < collision.transform.position.y)
        {
            // if isObstacle is unchecked, pressureplate activates moving platform
            if (transform.position.y > endPos.y)
            {
                transform.Translate(0, -0.01f, 0);
                back = false;
            }

            if (isObstacle)
            {
                obstacle.DisableObstacle();
            }
            else
            {
                terrain.ActivateMovingPlatform();
            }
        }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        back = true;
    }

    private void Update()
    {
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
    }
}
