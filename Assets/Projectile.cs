using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float initialVelocity = 10f;
    public float timeResolution = 0.02f;
    public float maxTime = 10f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {

        Vector3 velocityVector = transform.up * initialVelocity;

        lr.SetVertexCount((int)(maxTime / timeResolution));

        int index = 0;
        Vector3 curPos = transform.position;

        for (float t = 0f; t < maxTime; t += timeResolution)
        {
            lr.SetPosition(index, curPos);
            curPos += velocityVector * timeResolution;
            velocityVector += Physics.gravity * timeResolution;
            index++;
        }
    }
}
