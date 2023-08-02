using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private FadeInOut fade;

    private void Start()
    {
        fade = FindObjectOfType<FadeInOut>();
        fade.SetAlpha(1f);
        fade.FadeOut();
    }
}
