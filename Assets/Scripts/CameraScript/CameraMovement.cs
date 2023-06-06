using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private Vector2 mousePosition;
    private Vector2 playerPosition;


    //TEMP VARIABLE
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject target;

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
        target.transform.position = playerPosition + (mousePosition - playerPosition) * 0.2f;
    }
}
