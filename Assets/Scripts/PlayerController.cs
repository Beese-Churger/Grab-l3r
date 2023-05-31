using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject Grapple1;
    [SerializeField] private GameObject Grapple2;

    void Awake()
    {
        Grapple1 = GameObject.Find("Grapple1");
        Grapple2 = GameObject.Find("Grapple2");
    }

    private void Update()
    {
        GrappleUpdate();
    }

    private void GrappleUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        Debug.Log(mousePos);
        Vector3 Dir = (mousePos - this.gameObject.transform.position).normalized;

        float angle = angle360(Dir, Vector2.Angle(Dir, this.gameObject.transform.up));
        Debug.Log(angle);
        if (IsRight(angle))
        {
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("1");
            }
        }
        else 
        {
            if (Input.GetKey(KeyCode.S))
            {
                Debug.Log("2");
            }
        }


    }

    private bool IsRight(float angle)
    {
        if (angle > 180)
        {
            return false;
        }
        return true;
    }
    float angle360(Vector3 dir, float theangle) // calculate 360 angle using Vec3.Angle
    {
        float angle2 = Vector2.Angle(dir, this.gameObject.transform.right);
        if (angle2 > 90f)
        {
            theangle = 360f - theangle;
        }
        return theangle;
    }
}
