using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRope : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(6,6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
