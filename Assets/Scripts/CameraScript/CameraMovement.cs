using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Vector2 mousePosition;
    private Vector2 playerPosition;
    private Vector3 cameraPosition;

    private Camera camera1;


    //TEMP VARIABLE
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference pointer;
    [SerializeField] private bool boss = false;



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

        boss = GameManager.GetInstance().GetGameState() == StateType.boss ? true : false;
        Debug.Log(GameManager.GetInstance().GetGameState());

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

                transform.position = cameraPosition;
            }
        }
        else
        {
            camera1.orthographicSize = 12f;
            transform.position = new Vector3(0f, 0f, -10f);
        }
       
    }
}
