using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    public static SafeZoneManager instance = null;
    [SerializeField] private GameObject[] safeZoneArr;
    private Transform player;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    public bool CheckSafeZone()
    {
        foreach (GameObject safeZone in safeZoneArr)
        {
            if (safeZone.GetComponent<Collider2D>().OverlapPoint(player.position))
            {
                return true;
            }
        }
        return false;
    }
}
