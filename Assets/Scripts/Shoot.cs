using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {

    public GameObject player;
    public float playerKnockback = 8.5f;
    public float reloadTime;
    [Header("Red")]
    public string gunType = "Red";
    public GameObject redBullet;
    public float redSpeed = 20f;
    public float redReloadTime;
    public Image img;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(reloadTime>=0)
        {
            reloadTime -= Time.deltaTime;
        }
        else
        {
            reloadTime = 0;
        }
        
        if(Input.GetButtonDown("Shoot") && reloadTime == 0)
        {
            if(gunType == "Red")
            {
                GameObject an = Instantiate(redBullet, transform.position, transform.rotation)as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * redSpeed);
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * redSpeed * playerKnockback);
                reloadTime = redReloadTime;
                img.fillAmount = reloadTime / redReloadTime;
            }

        }
		
	}
}
