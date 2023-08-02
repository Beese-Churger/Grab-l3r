using UnityEngine;
public class Terrain : MonoBehaviour
{
    public enum TerrainType
    {
        concreate,
        vines,
        moving
    }
    [SerializeField] TerrainType terrainType;

    public float speed = 0.2f;
    public Vector2 endPos;
    public Animator animator;
    public bool triggerPressurePlate = false;

    private Vector2 startPos;
    private bool isRight;
    private float timer;
    private float delay = 2f;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Awake()
    {
        isRight = false;
    }

    void Update()
    {
        switch (terrainType)
        {
            case TerrainType.moving:
                if(triggerPressurePlate)
                {
                    Move();
                }
                break;
        }
    }

    // move moving platform when activated
    private void Move()
    {
        if (!isRight)
        {
            // move platform to the right and pause once target position reached
            if (Vector2.Distance(transform.position, endPos) < 0.001f)
            {
                transform.position = endPos;
                timer += Time.deltaTime;
                if(timer > delay)
                {
                    isRight = true;
                    timer = 0;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
            }
        }
        else
        {
            // move platform tothe left and pause once target position reached
            if (Vector2.Distance(transform.position, startPos) < 0.001f)
            {
                transform.position = startPos;
                timer += Time.deltaTime;
                if (timer > delay)
                {
                    isRight = false;
                    timer = 0;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            }
        }
        if (!GameObject.Find("movingplatform_AudioClip"))
        {
            AudioManager.Instance.PlaySFX("movingplatform", transform.position);
        }
        else if (!GameObject.Find("movingplatform_AudioClip").GetComponent<AudioSource>().isPlaying)
            GameObject.Find("movingplatform_AudioClip").GetComponent<AudioSource>().Play();
    }

    // activate moving platform
    public void ActivateMovingPlatform()
    {
        triggerPressurePlate = true;
        animator.SetBool("isActivated", true);
        AudioManager.Instance.PlaySFX("movingplatform_on", transform.position);
    }

    // deactivate moving platform
    public void DeactivateMovingPlatform()
    {
        triggerPressurePlate = false;
        animator.SetBool("isActivated", false);
        AudioManager.Instance.PlaySFX("movingplatform_off", transform.position);
        GameObject.Find("movingplatform_AudioClip").GetComponent<AudioSource>().Stop();

    }

    // get terrain type
    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
    public void SetTerrainType(TerrainType tType)
    {
        terrainType = tType;
    }
}