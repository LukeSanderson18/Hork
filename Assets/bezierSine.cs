using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bezierSine : MonoBehaviour {

    public float frequency = 20.0f;  // Speed of sine movement
    public float magnitude = 0.5f;   // Size of sine movement

    Vector2 firstpoint;
    Vector2 secondpoint;

	// Use this for initialization
	void Start () {

      //  firstpoint = GetComponent<BezierCollider2D>().handlerFirstPoint;
      //  secondpoint = GetComponent<BezierCollider2D>().handlerSecondPoint;
		
	}
	
	// Update is called once per frame
	void Update () {
        firstpoint = Vector3.up * Mathf.Sin(Time.time * frequency) * magnitude;
        GetComponent<BezierCollider2D>().handlerFirstPoint.y = firstpoint.y;
        GetComponent<BezierCollider2D>().handlerSecondPoint.y = -firstpoint.y;
    }
}
