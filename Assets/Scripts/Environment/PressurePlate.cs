using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            float d = Vector2.Distance(transform.position, collision.transform.position);
            if (d < 0.05f)
            {
                // TODO: deactivate obstacle
            }
        }
        Destroy(this);
    }
}
