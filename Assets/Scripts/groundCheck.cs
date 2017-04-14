using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour {

    public Legs mainSprite;
    Vector2 gravityDirection;
    public bool isGrounded;
    public float distance;
    public LayerMask lm;
    //public float  xOffset = 0.0f;
    //public float yOffset = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        gravityDirection = mainSprite.gravityDirection;
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x + -gravityDirection.x*0.5f, transform.position.y + -gravityDirection.y*0.5f), gravityDirection, distance, lm);
        Debug.DrawRay(new Vector2(transform.position.x + -gravityDirection.x*0.5f, transform.position.y + -gravityDirection.y*0.5f), gravityDirection, Color.green, distance);
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
