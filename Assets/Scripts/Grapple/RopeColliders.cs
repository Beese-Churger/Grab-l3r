using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
public class RopeColliders : MonoBehaviour
{
    LineRenderer rope;
    EdgeCollider2D edgeCollider;

    Vector3 points;
    Vector2[] points2 = new Vector2[35];

    private void Start()
    {
        edgeCollider = this.gameObject.AddComponent<EdgeCollider2D>();
        rope = this.gameObject.GetComponent<LineRenderer>();

        getNewPositions();

        edgeCollider.points = points2;
    }

    private void Update()
    {
        getNewPositions();

        edgeCollider.points = points2;
    }

    void getNewPositions()
    {
        for (int i = 0; i < rope.positionCount; i++)
        {
            points = rope.GetPosition(i);
            points2[i] = new Vector2(points.x, points.y);
        }
    }
}
