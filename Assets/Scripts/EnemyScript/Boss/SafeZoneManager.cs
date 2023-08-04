using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    public static SafeZoneManager instance = null;
    [SerializeField] private GameObject[] safeZoneArr;
    private Transform player;
    bool dir = true;
    Color originalColor;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            player = GameObject.FindGameObjectWithTag("Player").transform;
            originalColor = safeZoneArr[0].GetComponent<SpriteRenderer>().color;
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
    public void FlashZones(bool isFlashing)
    {
        //Debug.Log("Flash zones");
        foreach (GameObject safeZone in safeZoneArr)
        {
            if (isFlashing)
            {
                SpriteRenderer temp = safeZone.GetComponent<SpriteRenderer>();
                Color currentCol = temp.color;

                if (!temp.enabled)
                    temp.enabled = true;


                if (currentCol.a <= originalColor.a && dir)
                    currentCol.a += Time.deltaTime * 0.3f;
                else if (currentCol.a >= originalColor.a)
                    dir = false;
                if (currentCol.a >= 0.1f && !dir)
                    currentCol.a -= Time.deltaTime * 0.3f;
                else if (currentCol.a <= 0.1f)
                    dir = true;

                temp.color = currentCol;
            }
            else
            {
                SpriteRenderer temp = safeZone.GetComponent<SpriteRenderer>();
                temp.enabled = false;
            }

        }
    }
}
//if (isFlash)
//{
//    if (bossBeamColor.color.a <= 0.5f && dir)
//        beam.a += Time.deltaTime * 0.8f;
//    else if (bossBeamColor.color.a >= 0.5f)
//        dir = false;
//    if (bossBeamColor.color.a >= 0.1f && !dir)
//        beam.a -= Time.deltaTime * 0.8f;
//    else if (bossBeamColor.color.a <= 0.1f)
//        dir = true;
//}
//else
//    beam.a = 0.5f;

//bossBeamColor.color = beam;