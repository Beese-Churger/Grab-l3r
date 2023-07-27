using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer instance = null;
    public float time;
    [SerializeField] TMP_Text timer;
    private void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
        }
    }
    void Awake()
    {
        time = 0f; 
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        timer.text = formatTimer(time);
    }

    private string formatTimer(float currentTime)
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        return string.Format("{00:00} : {01:00}", minutes, seconds);
    }
}
