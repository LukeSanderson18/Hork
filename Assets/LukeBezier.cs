using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeBezier : MonoBehaviour {

    public int noOfSegments = 20;
    LineRenderer lr;
    public int noOfChildrenAtStart = 2;
    public GameObject nextGO;

    public Vector3 startPoint;
    public Vector3 startControlPoint;
    public Vector3 endPoint;
    public Vector3 endControlPoint;

    public Vector3 bezierPoint;
    // Use this for initialization
    void Start () {

        //      controlPoint = Vector3.Lerp(startPoint, endPoint, 0.5f);
        lr = GetComponent<LineRenderer>();


    }

    // Update is called once per frame
    void Update () {

        foreach (Transform child in transform)
        {

            int index = child.GetSiblingIndex();

            if (index != transform.childCount-1)
            {
                startPoint = child.transform.position;
                startControlPoint = child.GetChild(0).transform.position;
                endControlPoint = child.GetChild(1).transform.position;
                GameObject nextBrotherNode = transform.GetChild(index + 1).gameObject;
                endPoint = nextBrotherNode.transform.position;

                //lr.SetVertexCount(((transform.childCount*noOfSegments)-noOfSegments)+2);
                lr.SetVertexCount(41);
               // lr.SetVertexCount((transform.childCount * noOfSegments)-noOfSegments)+1);
                // print(lr.numCapVertices);
                if (index == 0)
                {
                    for (int i = 0; i < noOfSegments+1; i++)
                    {
                      //  print(i);
                        GetBezier(i / (float)(noOfSegments));
                        lr.SetPosition(i, bezierPoint);
                    }
                }
                else
                {
                    print(index);
                    for (int i = (noOfSegments * index)+1; i <= ((noOfSegments * index) + noOfSegments) + 1; i++)
                    {
                        //  print(i);
                        // if (i != (noOfSegments*index) - 1)
                        {
                            GetBezier(i / (float)(noOfSegments + (index * noOfSegments))-1);
                            lr.SetPosition(i, bezierPoint);
                        }
                    }
                }

            }
        }     

    }

    public Vector3 GetBezier(float t)
    {
        bezierPoint.x = (1 - t) * (1 - t) * (1 - t) * startPoint.x + 3 * (1 - t) * (1 - t) * t * startControlPoint.x + 3 * (1 - t) * t * t * endControlPoint.x + t * t * t * endPoint.x;
        bezierPoint.y = (1 - t) * (1 - t) * (1 - t) * startPoint.y + 3 * (1 - t) * (1 - t) * t * startControlPoint.y + 3 * (1 - t) * t * t * endControlPoint.y + t * t * t * endPoint.y;
        bezierPoint.z = 0;

        return bezierPoint;
    }
}
