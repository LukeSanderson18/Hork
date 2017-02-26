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

    [Space(10)]
    Color redColor = Color.red;
    public GameObject redBullet;
    public float redSpeed = 20f;
    public float redReloadTime;
    public int redDamage = 10;

    [Header("Orange")]
    Color orangeColor = new Color(1, 0.6f, 0, 1);
    public GameObject orangeBullet;
    public float orangeSpeed = 20f;
    public float orangeReloadTime;
    public float orangeCharge = 1f;
    public int orangeDamage = 20;

    [Header("Yellow")]
    Color yellowColor = Color.yellow;
    public GameObject yellowBullet;
    public float yellowSpeed = 20f;
    public float yellowReloadTime;
    public int yellowDamage = 3;

    [Header("Green")]
    Color greenColor = Color.green;
    public GameObject greenBullet;
    public float greenSpeed = 20f;
    public float greenReloadTime;
    public int greenDamage;

    [Header("Blue")]
    Color blueColor = Color.blue;
    public GameObject blueBullet;
    public float blueSpeed = 20f;
    public float blueReloadTime;
    public int blueDamage = 9;

    [Header("Purple")]
    Color purpleColor = new Color(0.541f, 0.168f, 0.886f, 1);
    public GameObject purpleBullet;
    public float purpleSpeed = 20f;
    public float purpleReloadTime;
    public int purpleDamage = 8;

    [Header("Pink")]
    Color pinkColor = new Color(1,0,0.56f,1);
    public GameObject pinkBullet;
    public float pinkSpeed = 20f;
    public float pinkReloadTime;
    public int pinkDamage;

    [Header("White")]
    Color whiteColor = Color.white;
    public GameObject whiteBullet;
    public float whiteSpeed = 20f;
    public float whiteReloadTime;
    public int whiteDamage;



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
            an.GetComponent<BulletDamage>().bulletDam = orangeDamage;
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
                an.GetComponent<BulletDamage>().bulletDam = redDamage;
                player.GetComponent<Rigidbody2D>().AddForce(-transform.up * redSpeed * playerKnockback);
                reloadTime -= redReloadTime;
            }
            if (gunType == "Blue")
            {
                GameObject an = Instantiate(blueBullet, transform.position, transform.rotation) as GameObject;
                an.GetComponent<Rigidbody2D>().AddForce(transform.up * blueSpeed);
                an.GetComponent<BulletDamage>().bulletDam = blueDamage;
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
                an.GetComponent<BulletDamage>().bulletDam = purpleDamage;
                an2.GetComponent<BulletDamage>().bulletDam = purpleDamage;
                an3.GetComponent<BulletDamage>().bulletDam = purpleDamage;


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
            proj.initialVelocity = (orangeSpeed * (orangeCharge * 0.6f) * 0.006f);
            //print("LINE ++ " + (orangeSpeed * (orangeCharge*0.6f))*0.01f);
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
        an.GetComponent<BulletDamage>().bulletDam = yellowDamage;
        player.GetComponent<Rigidbody2D>().AddForce(-transform.up * yellowSpeed * playerKnockback * 0.33f);
        reloadTime -= yellowReloadTime;

    }
}
