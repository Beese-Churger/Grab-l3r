using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    private Image image;
    [SerializeField] private Image border;
    private GameManager instanceRef;

    private float maxVal, currVal;
    void Start()
    {
        instanceRef = GameManager.instance;
        image = GetComponent<Image>();

        int HPMaxValRef, HPCurValRef;
        instanceRef.GetPlayerHealth(out HPMaxValRef, out HPCurValRef);

        maxVal = HPMaxValRef;
        currVal = HPCurValRef;

    }

    // Update is called once per frame
    void Update()
    {
        if (image)
        {
            currVal = instanceRef.GetCurrentPlayerHealth();

            image.fillAmount = currVal / maxVal;

            if(currVal >= 4)
            {
                image.color = new Color32(0, 255, 0, 255);
                border.color = new Color32(0, 255, 0, 255);
            }
            else if(currVal < 4 && currVal > 1)
            {
                image.color = new Color32(255, 186, 0, 255);
                border.color = new Color32(255, 186, 0, 255);
            }
            else if (currVal <= 1)
            {
                image.color = new Color32(202, 46, 0, 255);
                border.color = new Color32(202, 46, 0, 255);
            }
        }
    }

}
