using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator.SetBool("collected",false);
    }

    public void PickUpEnded(string text)
    {
        if(text == "Done")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player has triggered checkpoint
        if (collision.transform.tag == "Player")
        {
            GameManager.instance.SetCollectables(1);
            animator.SetBool("collected", true);
        }
        //Destroy(this);
    }
}
