using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.transform.tag == "Player")
        {
            // set players last checkpoint position in player script
            //GameManager.instance.SetCheckPoint(transform.position);
            TestPlayer.checkpointPos = transform.position;

        }
        Destroy(this);
    }
}
