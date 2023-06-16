using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// I'm testing motion blur with this script because my assumption is that motion blur
// only works with the player movement and not the camera
public class TEMPSCRIPT : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;
    [SerializeField] private InputActionReference movement, grapple, jump;
    float v, h;
    Vector2 movementVec;
    public float speed;  // units per second; consider making this a public property for easy tweaking
    float dy, dx;

    // Update is called once per frame
    void Update()
    {
        

        movementVec = movement.action.ReadValue<Vector2>();


        if (jump.action.triggered)
        {
           Debug.Log("Dashed");
           m_Rigidbody.AddForce(transform.up * 15f, ForceMode2D.Impulse);
        }


        if (m_Rigidbody.velocity.magnitude < 5)
            m_Rigidbody.AddForce(movementVec, ForceMode2D.Impulse);


    }
}
