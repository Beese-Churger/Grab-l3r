using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        spikes,
        electricity,
        door,
        water
    }
    [SerializeField] ObstacleType obstacleType;
    public Animator animator;

    private Collider2D myCollider;
    private bool isActive;

    private void Awake()
    {
        isActive = true;
    }

    private void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
    }

    // cause damage to player on collision with obstacles
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            if (collision.gameObject.name == "Player" & obstacleType != ObstacleType.door)
            {
                // water objects are instant death to player
                if (obstacleType == ObstacleType.water)
                {
                    GameManager.instance.InstantDeath();
                }
                else
                {
                    GameManager.instance.TakeDamage();
                }
            }
        }
    }

    public void DisableObstacle()
    {
        isActive = false;
    }

    public void ActivateObstacle()
    {
        isActive = true;
    }

    public void OpenDoor()
    {
        animator.SetBool("isOpen", true);
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
        AudioManager.Instance.PlaySFX("door_open");
    }

    public void CloseDoor()
    {
        animator.SetBool("isOpen", false);
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }
        AudioManager.Instance.PlaySFX("door_close");
    }

    public void DeactivateElectricity()
    {
        animator.SetBool("Active", false);
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
    }

    public void ActivateElectricity()
    {
        animator.SetBool("Active", true);
        myCollider.enabled = true;
    }

    public ObstacleType GetObstacleType()
    {
        return obstacleType;
    }

    private void Update()
    {
        if (isActive)
        {
            switch (obstacleType)
            {
                case ObstacleType.electricity:
                    animator.SetBool("Active", true);
                    myCollider.enabled = true;
                    break;
            }
        }
        else
        {
            switch (obstacleType)
            {
                case ObstacleType.electricity:
                    animator.SetBool("Active", false);
                    if (myCollider != null)
                    {
                        myCollider.enabled = false;
                    }
                    break;
            }
        }
    }
}
