using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChange : MonoBehaviour
{
    FadeInOut fade;
    // change level once player triggers level end area
    private void Awake()
    {
        fade = FindObjectOfType<FadeInOut>();
    }

    public IEnumerator ChangeScene()
    {
        fade.FadeIn();
        yield return new WaitForEndOfFrame();
        LevelManager.instance.LoadNextLevel();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ChangeScene());
            //GameManager.instance.SetGameState(StateType.levelChange);
            //Destroy(this);
        }
    }
}
