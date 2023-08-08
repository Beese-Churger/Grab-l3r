using UnityEngine;

public class ExpandLeg : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.y *= GameManager.instance.GetCollectables();
        transform.localScale = currentScale;
    }

}
