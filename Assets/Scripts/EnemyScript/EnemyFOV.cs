using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Detected Player");
            this.transform.parent.gameObject.GetComponent<SmallEnemy>().SetState(2);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.transform.parent.gameObject.GetComponent<SmallEnemy>().SetState(1);
        }

    }
}
