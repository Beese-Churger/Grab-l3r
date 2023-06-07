using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I'm testing motion blur with this script because my assumption is that motion blur
// only works with the player movement and not the camera
public class TEMPSCRIPT : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;
    float v, h;
    public float speed;  // units per second; consider making this a public property for easy tweaking
    float dy, dx;
    // Update is called once per frame
    void Update()
    {
        

        v = Input.GetAxis("Vertical");
        dy = v * speed * Time.deltaTime;

        h = Input.GetAxis("Horizontal");
        dx = h * speed * Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Space))
        {
           Debug.Log("Dashed");
            //dy = 100 * speed * Time.deltaTime;
            m_Rigidbody.AddForce(transform.right * 10.0f, ForceMode2D.Impulse);
        }




        transform.position = new Vector2(transform.position.x + dx, transform.position.y+dy);
        
 
    }
}