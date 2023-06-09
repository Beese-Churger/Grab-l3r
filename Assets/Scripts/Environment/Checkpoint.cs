using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // check player collision with checkpoint sprite, set player position to checkpoint position for respawn
    // called only once
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.transform.tag == "Player")
        {
            // set players last checkpoint position in player script
            Player.checkpointPos = transform.position;
        }
        Destroy(this);
    }
}
