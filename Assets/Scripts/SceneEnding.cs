using UnityEngine;
using UnityEngine.Video;

public class SceneEnding : MonoBehaviour
{
    // Start is called before the first frame update
    public VideoPlayer vid;


    void Start()
    {
        vid.loopPointReached += CheckOver;
    }

    void CheckOver(VideoPlayer vp)
    {
        print("Video Is Over");
        LevelManager.instance.LoadNextLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
            CheckOver(vid);
    }
}
