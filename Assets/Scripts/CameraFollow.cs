using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject player;
    Rigidbody2D rb;
    float hor;
    float ver;
    public float height = 3;
    public float multi = 2;
    public float speed = 1;
	// Use this for initialization
	void Start () {
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hor = Input.GetAxis("RHor");
        ver = Input.GetAxis("RVer");
        transform.position = Vector3.Lerp(transform.position, player.transform.position + (new Vector3((hor * multi)+(rb.velocity.x*0.3f), height + (-ver * multi), -10)), Time.fixedDeltaTime * speed);
    }
}
