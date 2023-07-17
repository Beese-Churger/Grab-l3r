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
        //add reference
        //ThermalCamSwap.instance.AddToThermalSwapList(gameObject);

       // updateThermalMat();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateMat(int state)
    {
        matState = state;
        //if (ThermalCamSwap.instance)
        //{
        //    matState = ThermalCamSwap.instance.GetThermalState();
        //}

        //defualt
        if (matState == 0)
        {
            if (ToggleMaterials[0] != null)
            {
                if (renderer != null)
                {
                    renderer.material = ToggleMaterials[0];
                }
                //else
                //{
                //    Renderer[] rendList = GetComponentsInChildren<Renderer>();

                //    foreach (Renderer rendererRef in rendList)
                //    {
                //        if (rendererRef == GetComponentInChildren<ParticleSystem>())
                //        {

                //        }
                //        else
                //        {
                //            rendererRef.material = ToggleMaterials[0];
                //        }
                //    }
                //}
            }
        }
        else if (matState == 1) //damage taken
        {
            if (ToggleMaterials[1] != null)
            {
                if (renderer != null)
                {
                    renderer.material = ToggleMaterials[1];
                }
                //else
                //{
                //    Renderer[] rendList = GetComponentsInChildren<Renderer>();

                //    foreach (Renderer rendererRef in rendList)
                //    {
                //        rendererRef.material = ToggleMaterials[1];
                //    }
                //}
            }
        }
        //else if (matState == 2) //thermal black
        //{
        //    if (ToggleMaterials[2] != null)
        //    {
        //        if (renderer != null)
        //        {
        //            renderer.material = ToggleMaterials[2];
        //        }
        //        else
        //        {
        //            Renderer[] rendList = GetComponentsInChildren<Renderer>();

        //            foreach (Renderer rendererRef in rendList)
        //            {
        //                rendererRef.material = ToggleMaterials[2];
        //            }
        //        }
        //    }
        //}
    }

}