using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour {

    public bool isGrounded;
    public float distance;
    public LayerMask lm;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -Vector2.up, distance, lm);
        if (hitLeft.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }
}
