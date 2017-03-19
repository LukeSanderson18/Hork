using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    public Transform BG1;
    public float multiplier = 0.2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        BG1.transform.position = new Vector2(transform.position.x * multiplier, BG1.transform.position.y);
		
	}
}
