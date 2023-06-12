using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    [SerializeField] private InputActionReference pointer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(pointer.action.ReadValue<Vector2>());
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        transform.position = mousePosition;
    }
}
