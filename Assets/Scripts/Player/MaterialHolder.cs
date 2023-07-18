using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHolder : MonoBehaviour
{
    // Start is called before the first frame update
    public Material[] ToggleMaterials;
    private Renderer renderer;

    public int matState = 0;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateMat(int state)
    {
        matState = state;

        //defualt
        if (matState == 0)
        {
            if (ToggleMaterials[0] != null)
            {
                if (renderer != null)
                {
                    renderer.material = ToggleMaterials[0];
                    Debug.Log("test");
                }
            }
        }
        else if (matState == 1) //damage taken
        {
            if (ToggleMaterials[1] != null)
            {
                if (renderer != null)
                {
                    renderer.material = ToggleMaterials[1];
                    Debug.Log("test2");
                }
            }
        }
    }
}