using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    private Vector2 mousePosition;
    private Vector2 playerPosition;
    private Vector2 lookAtPosition;
    private CinemachineVirtualCamera virtualCamera;


    //TEMP VARIABLE
    private GameObject playerPlaceHolder;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        // keep cursor confined in the game window
        Cursor.lockState = CursorLockMode.Confined;
        // Get Component Of CinemachineVirtualCamera
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        // Find PlayerPlaceHolderObject
        playerPlaceHolder = GameObject.Find("PlayerPlaceHolder");
        // Find Target
        target = GameObject.Find("CameraTarget");

        // Initialize Mouse Position
        mousePosition = new Vector2(0,0);

        // Initialize Player Position
        playerPosition = playerPlaceHolder.transform.position;

    }

     
    void FixedUpdate()
    {
         // Converted from screen space to world space
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Update player position
        playerPosition = playerPlaceHolder.transform.position;


    }

    // Update is called once per frame
    void LateUpdate()
    {
        // The Position Of The Invisible Target
        lookAtPosition = Vector2.Lerp(playerPosition, mousePosition, 0.5f);
        // Set the position of the invisible target
        target.transform.position = lookAtPosition;
        //Debug.Log(target.transform.position.x + "," + target.transform.position.y);

    }
}
