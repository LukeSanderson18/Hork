using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {

    public GameObject player;
    public float playerKnockback = 8.5f;
    public float reloadTime;
    public string gunType = "Red";

    [Header("Red")]
    Color redColor = Color.red;
    public GameObject redBullet;
    public float redSpeed = 20f;
    public float redReloadTime;

    [Header("Purple")]
    Color purpleColor = new Color(0.541f, 0.168f, 0.886f, 1);
    public GameObject purpleBullet;
    public float purpleSpeed = 20f;
    public float purpleReloadTime;


    public Image img;
    float oddfloat = 0;

	
	// Update is called once per frame
	void Update () {

        img.fillAmount = reloadTime;

        if (img.fillAmount == 1)
        {
            img.transform.localScale = Vector2.Lerp(img.transform.localScale, Vector2.one * 2, Time.deltaTime * 4);
            oddfloat += Time.deltaTime * 2;
        }
        else
        {
            img.transform.localScale = Vector2.Lerp(img.transform.localScale, Vector2.one, Time.deltaTime * 12);
            oddfloat = 0;
        }

        if (reloadTime<1)
        {
            reloadTime += Time.deltaTime;
        }
        else
        {
            reloadTime = 1;
        }
        
        if(Input.GetButtonDown("Shoot") && reloadTime > 0)
        {
            if(gunType == "Red")
            {
                GameObject an = Instantiate(redBullet, transform.position, transform.rotation)as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * redSpeed);
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * redSpeed * playerKnockback);
                reloadTime -= redReloadTime;
            }
            if (gunType == "Purple")
            {
                GameObject an = Instantiate(purpleBullet, transform.position, transform.rotation) as GameObject;
                GameObject an2 = Instantiate(purpleBullet, transform.position, transform.rotation) as GameObject;
                GameObject an3 = Instantiate(purpleBullet, transform.position, transform.rotation) as GameObject;

                an2.transform.Rotate(0, 0, -30);
                an3.transform.Rotate(0, 0, 30);

                an.GetComponent<Rigidbody2D>().AddForce(an.transform.up * purpleSpeed);
                an2.GetComponent<Rigidbody2D>().AddForce(an2.transform.up * purpleSpeed);
                an3.GetComponent<Rigidbody2D>().AddForce(an3.transform.up * purpleSpeed);

                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * purpleSpeed * playerKnockback);
                reloadTime -= purpleReloadTime;
            }
        }

        if (gunType == "Red")
        {
            if(img.fillAmount == 1)
            {
                img.color = Color.Lerp(redColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = redColor;
            }
        }

        if (gunType == "Purple")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(purpleColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = purpleColor;
            }
        }

    }
}
