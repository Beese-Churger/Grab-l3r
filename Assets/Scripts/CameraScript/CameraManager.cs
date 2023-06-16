using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleCamera;

    public Camera camera1; // Reference to the first camera
    public Camera camera2; // Reference to the second camera

    private bool isCamera1Active = true; // Flag to keep track of the active camera

    void Start()
    {
        // Enable the first camera and disable the second camera at the start
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {
        // Check for the toggle input (e.g., a button press, key press, etc.)
        if (toggleCamera.action.triggered)
        {
            // Toggle the active camera
            isCamera1Active = !isCamera1Active;

            // Enable/disable the cameras accordingly
            camera1.enabled = isCamera1Active;
            camera2.enabled = !isCamera1Active;
        }
    }
}
