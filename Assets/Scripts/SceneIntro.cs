using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SceneIntro : MonoBehaviour
{
    public VideoPlayer vid;


    void Start() 
    { 
        vid.loopPointReached += CheckOver; 
    }

    void CheckOver(VideoPlayer vp)
    {
        print("Video Is Over");
        GameManager.instance.SetGameState(StateType.levelChange);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
            CheckOver(vid);
    }
}
