using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Animator animator;

    // check if player has collected 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.gameObject.name == "Player")
        {
            // set collectables at game manager and play animation
            GameManager.instance.SetCollectables(1);
            animator.SetBool("collected", true);
        }
    }

    // Check if collect animation has ended to destroy the game object
    public void PickUpEnded(string text)
    {
        // pickUpEnded emits done when animation has been played
        if (text == "Done")
        {
            Destroy(this.gameObject);
        }
    }
}
