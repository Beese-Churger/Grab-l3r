using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance = null;
    private Vector2 mousePosition;
    private Vector2 playerPosition;
    private Vector3 cameraPosition;

    private Vector3 cameraOffset;

    private Camera camera1;
    public float smoothSpeed = 0.125f;
    private const float xOffset = 10f;
    private const float yOffset = 10f;
    private float edgeSizeX = Screen.width * 0.3f;
    private float leftEdgeX = Screen.width * 0.1f;
    private float topEdgeY = Screen.height * 0.2f;
    private float edgeSizeY = Screen.height * 0.2f;
    //TEMP VARIABLE
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference pointer;
    private bool boss = false;

    private bool move = false;
    private bool switchMode = false;


    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
           // DontDestroyOnLoad(instance);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        camera1 = GetComponent<Camera>();
        // keep cursor confined in the game window
        Cursor.lockState = CursorLockMode.Confined;

        // Initialize Mouse Position
        mousePosition = new Vector2(0,0);

        // Initialize Player Position
        playerPosition = player.transform.position;

        cameraOffset = transform.position;
        //transform.position = new Vector3(playerPosition.x, playerPosition.y, -10f);
        
        boss = GameManager.instance.GetLevelManager().GetCurrentLevel() == "LevelLayout Boss";
       // Debug.Log(GameManager.GetInstance().GetGameState());

    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    switchMode = !switchMode;
        //}
    }
    void FixedUpdate()
    {
        if (!boss)
        {
            if (camera1.enabled)
            {
                // Converted from screen space to world space
                mousePosition = Camera.main.ScreenToWorldPoint(pointer.action.ReadValue<Vector2>());

                // convert vector3 to vector2
                playerPosition = player.transform.position;

                // The Position Of The Invisible Target
                cameraPosition = playerPosition + (mousePosition - playerPosition) * 0.2f;
                cameraPosition.z = -10.0f;

                if (switchMode)
                {
                    // Check if player is near the edge of the screen
                    if ((Camera.main.WorldToScreenPoint(playerPosition).x > Screen.width - edgeSizeX ||
                         Camera.main.WorldToScreenPoint(playerPosition).x < leftEdgeX ||
                         Camera.main.WorldToScreenPoint(playerPosition).y > Screen.height - topEdgeY ||
                         Camera.main.WorldToScreenPoint(playerPosition).y < edgeSizeY) ||
                         move)
                    {
                        FollowPlayer();
                    }
                    else
                    {
                        //Vector2 newPPos = playerPosition + new Vector2(0f, 5f);
                        //if (Mathf.Abs(cameraOffset.y - newPPos.y) > 1)
                        //{
                        //    Vector2 dir = ((Vector2)cameraOffset - newPPos).normalized;
                        //    cameraOffset.y += yOffset * Time.deltaTime * dir.y;
                        //    transform.position = cameraOffset;
                        //}
                    }
                }
                else
                {
                    transform.position = cameraPosition;
                }
            }
        }
        else
        {
            camera1.orthographicSize = 12f;
            //transform.position = new Vector3(0f, 0f, -10f);
        }
       
    }
    private void FollowPlayer()
    {
        Vector2 direction = ((Vector2)cameraOffset - playerPosition).normalized;
        Vector2 newPPos = playerPosition + new Vector2(5f, 8f * direction.y);
        Vector2 dir = (newPPos - (Vector2)cameraOffset).normalized;
        if (Mathf.Abs(cameraOffset.x - newPPos.x) > 1)
        {
            move = true;
            cameraOffset.x += xOffset * Time.deltaTime * dir.x;
        }
        if (Mathf.Abs(cameraOffset.y - newPPos.y) > 1)
        {
            move = true;
            cameraOffset.y += yOffset * Time.deltaTime * dir.y;
        }
        else
            move = false;

        cameraOffset.z = -10f;
        transform.position = cameraOffset;

    }
    public void SetCameraState()
    {
        if (GameManager.GetInstance().GetLevelManager().GetCurrentLevel() == "LevelLayout Boss")
        {
            boss = true;
        }
        else
            boss = false;
    }
}
