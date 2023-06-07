using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] public Terrain terrain;
    [SerializeField] public Obstacle obstacle;
    private Vector2 startPos;
    public Vector2 endPos;
    public bool isObstacle;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (isObstacle)
            {
                obstacle.DisableObstacle();
            }
            else
            {
                if (transform.position.y > endPos.y)
                {
                    transform.Translate(0, -0.01f, 0);
                }
                terrain.ActivateMovingPlatform();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
