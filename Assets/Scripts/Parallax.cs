using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool scrollLeft;
    
    private GameObject player;
    private Rigidbody2D rbody;

    float singleTextureWidth;
    private void Awake()
    {
        SetupTexture();
        if (scrollLeft)
            moveSpeed = -moveSpeed;

        player = GameObject.Find("Player");
        rbody = player.GetComponent<Rigidbody2D>();

    }
    void SetupTexture()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        singleTextureWidth = sprite.texture.width / sprite.pixelsPerUnit;
    }

    void Scroll()
    {
        if (!player)
            return;
        float x = (rbody.velocity.x * Time.deltaTime) / moveSpeed;
        float y = (rbody.velocity.y * Time.deltaTime) / moveSpeed;
        transform.position += new Vector3(-x, 0f, 0f);
    }

    void CheckReset()
    {
        if((Mathf.Abs(transform.position.x) - singleTextureWidth) > 0)
        {
            transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
        }
    }

    private void Update()
    {
        Scroll();
        CheckReset();
    }
}
