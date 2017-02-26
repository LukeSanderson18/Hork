using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {

    public Projectile proj;
    public WeaponWheel ww;
    public GameObject player;
    public float playerKnockback = 8.5f;
    public float reloadTime;
    public string gunType = "Red";

    [Header("Red")]
    Color redColor = Color.red;
    public GameObject redBullet;
    public float redSpeed = 20f;
    public float redReloadTime;

    [Header("Orange")]
    Color orangeColor = new Color(1, 0.6f, 0, 1);
    public GameObject orangeBullet;
    public float orangeSpeed = 20f;
    public float orangeReloadTime;
    public float orangeCharge = 1f;

    [Header("Yellow")]
    Color yellowColor = Color.yellow;
    public GameObject yellowBullet;
    public float yellowSpeed = 20f;
    public float yellowReloadTime;

    [Header("Green")]
    Color greenColor = Color.green;
    public GameObject greenBullet;
    public float greenSpeed = 20f;
    public float greenReloadTime;

    [Header("Blue")]
    Color blueColor = Color.blue;
    public GameObject blueBullet;
    public float blueSpeed = 20f;
    public float blueReloadTime;

    [Header("Purple")]
    Color purpleColor = new Color(0.541f, 0.168f, 0.886f, 1);
    public GameObject purpleBullet;
    public float purpleSpeed = 20f;
    public float purpleReloadTime;

    [Header("Pink")]
    Color pinkColor = new Color(1,0,0.56f,1);
    public GameObject pinkBullet;
    public float pinkSpeed = 20f;
    public float pinkReloadTime;

    [Header("White")]
    Color whiteColor = Color.white;
    public GameObject whiteBullet;
    public float whiteSpeed = 20f;
    public float whiteReloadTime;



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

        
        if(Input.GetButton("Shoot") && gunType == "Orange" && reloadTime > 0)
        {
            orangeCharge += Time.deltaTime*2;
        }
        if (Input.GetButtonUp("Shoot") && gunType == "Orange" && reloadTime > 0)
        {
            if (orangeCharge >= 5)
            {
                orangeCharge = 5;
            }

            GameObject an = Instantiate(orangeBullet, transform.position, transform.rotation) as GameObject;
            an.GetComponent<Rigidbody2D>().AddForce(transform.up * orangeSpeed * (orangeCharge*0.6f));
            player.GetComponent<Rigidbody2D>().AddForce(-transform.up * orangeSpeed * playerKnockback);
            reloadTime -= orangeReloadTime;
            orangeCharge = 1f;
        }
        if(Input.GetButtonDown("Shoot") && reloadTime > 0)
        {
            if (gunType == "Red")
            {
                GameObject an = Instantiate(redBullet, transform.position, transform.rotation) as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * redSpeed);
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * redSpeed * playerKnockback);
                reloadTime -= redReloadTime;
            }
            if (gunType == "Blue")
            {
                GameObject an = Instantiate(blueBullet, transform.position, transform.rotation) as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * blueSpeed);
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * blueSpeed * playerKnockback);
                reloadTime -= blueReloadTime;
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
            if (gunType == "Yellow")
            {
                Invoke("YellowBullet",0);
                Invoke("YellowBullet", 0.08f);
                Invoke("YellowBullet", 0.16f);

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
        if (gunType == "Orange")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(orangeColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = orangeColor;
            }

            proj.gameObject.SetActive(true);
            proj.initialVelocity = (orangeSpeed * (orangeCharge * 0.6f) * 0.01f);
            print((orangeSpeed * (orangeCharge*0.6f))*0.01f);
        }
        else
        {
            proj.gameObject.SetActive(false);
        }
        if (gunType == "Yellow")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(yellowColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = yellowColor;
            }
        }
        if (gunType == "Green")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(greenColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = greenColor;
            }
        }
        if (gunType == "Blue")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(blueColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = blueColor;
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
        if (gunType == "Pink")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(pinkColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = pinkColor;
            }
        }
        if (gunType == "White")
        {
            if (img.fillAmount == 1)
            {
                img.color = Color.Lerp(whiteColor, new Color(1, 1, 1, 0), oddfloat);
            }
            else
            {
                img.color = whiteColor;
            }
        }

        

    }

    void YellowBullet()
    {

        GameObject an = Instantiate(yellowBullet, transform.position, transform.rotation) as GameObject;
        an.transform.Rotate(0, 0, Random.Range(-15f, 15f));
        an.GetComponent<Rigidbody2D>().AddForce(an.transform.up * yellowSpeed);
        player.GetComponent<Rigidbody2D>().AddForce(-transform.up * yellowSpeed * playerKnockback * 0.33f);
        reloadTime -= yellowReloadTime;

    }
}
