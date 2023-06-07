using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (EnemyManager.enemyManager.GetEnemyType(transform.parent.gameObject) == 0)
            {
                Debug.Log("Small Enemy Detected Player");
                transform.parent.gameObject.GetComponent<SmallEnemy>().SetState(2);
            }
            else if (EnemyManager.enemyManager.GetEnemyType(transform.parent.gameObject) == 1)
            {
                Debug.Log("Big Enemy Detected Player");
                transform.parent.gameObject.GetComponent<BigEnemy>().SetState(2);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (EnemyManager.enemyManager.GetEnemyType(transform.parent.gameObject) == 0)
            {
                Debug.Log("Small Enemy stopped chasing the player");
                transform.parent.gameObject.GetComponent<SmallEnemy>().SetState(1);
            }
            else if (EnemyManager.enemyManager.GetEnemyType(transform.parent.gameObject) == 1)
            {
                Debug.Log("Big Enemy stopped chasing the player");
                transform.parent.gameObject.GetComponent<BigEnemy>().SetState(1);
            }
        }

    }
}
