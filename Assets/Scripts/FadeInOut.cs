using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool fadein = false;
    private bool fadeout = false;

    private float TimeToFade = 1f;

    private void Start()
    {
        canvasGroup = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
    }
    void Update()
    {
        canvasGroup = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
        if(fadein)
        {
            if(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += TimeToFade * Time.deltaTime;
                if(canvasGroup.alpha >= 1)
                {
                    fadein = false;
                }
            }
        }

        if (fadeout)
        {   
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= TimeToFade * Time.deltaTime;
                if (canvasGroup.alpha == 0)
                {
                    fadeout = false;
                }
            }
        }
    }
    public void SetAlpha(float i)
    {
        canvasGroup = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
        canvasGroup.alpha = i;
    }
    public void FadeIn()
    {
        fadein = true;
    }

    public void FadeOut()
    {
        fadeout = true;
    }
}
