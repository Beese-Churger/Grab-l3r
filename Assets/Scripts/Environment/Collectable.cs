using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.transform.tag == "Player")
        {
            GameManager.instance.SetScore(1);
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            sprite.color = Color.red;
        }
        Destroy(this.gameObject);
    }
}
