using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public RectTransform creditsTransform;
    public float transitionDelay = 3f; // Set the delay before transitioning to the next scene.
    public float creditsDuration = 60f; // Set the duration of the credits scene.

    private float totalCreditsHeight;
    private float visibleHeight;
    private float elapsedTime = 0f;
    private bool isTransitioning = false;

    private void Start()
    {
        totalCreditsHeight = creditsTransform.transform.parent.GetComponent<RectTransform>().rect.height + creditsTransform.rect.height;
        visibleHeight = Camera.main.orthographicSize * 2f;
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        while (true)
        {
            if (!isTransitioning)
            {
                creditsTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
                if (creditsTransform.anchoredPosition.y >= totalCreditsHeight)
                {
                    isTransitioning = true;
                    elapsedTime = 0f;
                }
            }
            yield return null;
        }
    }
    private void Update()
    {
        if (isTransitioning)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= transitionDelay)
            {
                LevelManager.instance.LoadNextLevel();
            }
        }
    }
}
