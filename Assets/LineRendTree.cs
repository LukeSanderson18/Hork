using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendTree : MonoBehaviour
{

    public Transform oneTran;
    public Transform twoTran;
    LineRenderer lr;
    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        for (int i = 0; i < GetComponent<BezierCollider2D>().pointsQuantity; i++)
        {
            lr.SetPosition(i, GetComponent<BezierCollider2D>().points[i]);

        }
    }

    // Update is called once per frame
    void LateUpdate()
    {



    }
}
