using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EdgeCollider2D))]
public class BezierCollider2D : MonoBehaviour
{
    public List<Vector2> points = new List<Vector2>();

    public Vector2 firstPoint;
    public Vector2 secondPoint;

    public Vector2 handlerFirstPoint;
    public Vector2 handlerSecondPoint;

    public int pointsQuantity;

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 handlerP0, Vector3 handlerP1, Vector3 p1)
    {
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term
        p += 3f * uu * t * handlerP0; //second term
        p += 3f * u * tt * handlerP1; //third term
        p += ttt * p1; //fourth term

        return p;
    }

    public Vector2[] calculate2DPoints()
    {
        points.Clear();
        points.Add(firstPoint);
        for (int i = 1; i < pointsQuantity; i++)
        {
            points.Add(CalculateBezierPoint((1f / pointsQuantity) * i, firstPoint, handlerFirstPoint, handlerSecondPoint, secondPoint));
            //do shit here you cretin
        }
        points.Add(secondPoint);
        print("reached");
        print(points[0]);

        return points.ToArray();


    }

}