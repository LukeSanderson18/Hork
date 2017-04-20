using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketAddition : MonoBehaviour {

    public Transform player;
    public float multi = 2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        transform.position = new Vector2(player.transform.position.x + (hor * multi), player.transform.position.y + (ver * multi));
		
	}
}
