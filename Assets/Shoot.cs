using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public GameObject player;
    public float playerKnockback = 8.5f;
    public string gunType = "Red";
    public GameObject redBullet;
    public float redSpeed = 20f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Shoot"))
        {
            if(gunType == "Red")
            {
                GameObject an = Instantiate(redBullet, transform.position, transform.rotation)as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * redSpeed);
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * redSpeed * playerKnockback);
            }

        }
		
	}
}
