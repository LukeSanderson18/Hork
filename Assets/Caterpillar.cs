using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : MonoBehaviour {

    public GameObject square;
    public GameObject leftTarget;
    public GameObject rightTarget;
    Rigidbody2D rb;
    public float moveSpeed;
    public bool firstOne;
	// Use this for initialization

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        }
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Floor"));
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            square.transform.position = new Vector2(transform.position.x, (transform.position.y - distance) + 0.7f);
            leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, square.transform.position.y - 0.7f);
            rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, square.transform.position.y - 0.7f);

        }

        if(firstOne)
        rb.AddForce(-Vector3.right  * moveSpeed * Time.fixedDeltaTime);
    }
}
