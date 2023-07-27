using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.gameObject.name == "Player")
        {
            // set player checkpoint
            var player = GameObject.Find("Player").GetComponent<SimpleController>();
            player.SetCheckPoint(transform.position);

            // activate checkpoint animation
            animator.SetBool("Activated", true);
        }
    }
}
