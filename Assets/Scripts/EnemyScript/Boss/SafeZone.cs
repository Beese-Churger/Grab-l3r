using UnityEngine;
// NOT IN USE
public class SafeZone : MonoBehaviour
{
    public bool playerSafe = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerSafe = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerSafe = false;
    }
}
