using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private Vector2 mousePosition;
    private Vector2 playerPosition;
    private Vector3 cameraPosition;



    //TEMP VARIABLE
    [SerializeField] private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        // keep cursor confined in the game window
        Cursor.lockState = CursorLockMode.Confined;

        // Initialize Mouse Position
        mousePosition = new Vector2(0,0);

        // Initialize Player Position
        playerPosition = player.transform.position;

    }

     
    void FixedUpdate()
    {
         // Converted from screen space to world space
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // convert vector3 to vector2
        playerPosition = player.transform.position;

        // The Position Of The Invisible Target
        cameraPosition = playerPosition + (mousePosition - playerPosition) * 0.2f;
        cameraPosition.z = -10.0f;

        transform.position = cameraPosition;
    }
}
