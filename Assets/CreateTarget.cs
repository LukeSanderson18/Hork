using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTarget : MonoBehaviour {

    public GameObject prefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 1"))
        {
            Instantiate(prefab, transform.position, transform.rotation);
        }
	}
}
