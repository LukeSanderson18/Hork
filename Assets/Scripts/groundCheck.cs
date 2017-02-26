using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour {

    public bool isGrounded;
    public float distance;
    public LayerMask lm;
    public float yOffset = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + yOffset), -Vector2.up, distance, lm);

        //if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), -Vector2.up, distance)) 
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
