using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.transform.CompareTag("Player"))
        {
            var player = GameObject.Find("Player").GetComponent<SimpleController>();
            player.SetCheckPoint(transform.position);

            animator.SetBool("Activated", true);
        }
        Destroy(this);
    }
}
