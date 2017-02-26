using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class WeaponWheel : MonoBehaviour
{
    public Legs legs;
    public GameObject beak;
    public GameObject mainCam;
    public Camera cam;

    public Rect bigRect = new Rect(0f, 0.1f, 0.6f, 0.85f);
    public Rect smallRect = new Rect(0.01f, 0.77f, 0.16f, 0.58f);

    Vector3 normRot = new Vector3(0, -60, 0);
    Vector3 bigRot = new Vector3(0, -90, 0);
    public float camSmooth = 1f;
    public float camRotSmooth = 10f;
    public float camTranSmooth = 2f;
    float startX;
    public float desired;
    public float smooth = 0.3f;
    private Quaternion qTo = Quaternion.identity;
    float t;
    public List<Transform> babies;
    bool pressed;

    public int i;

    // Use this for initialization
    void Start()
    {
        startX = transform.eulerAngles.x;
        desired = startX;

    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (Input.GetButton("Weapon1"))
        {
            legs.canTakeInput = false;
            legs.hor = 0;
            legs.ver = 0;

            mainCam.GetComponent<BlurOptimized>().enabled = true;

            cam.rect = new Rect(Mathf.Lerp(cam.rect.x, bigRect.x, Time.deltaTime * camSmooth),
                Mathf.Lerp(cam.rect.y, bigRect.y, Time.deltaTime * camSmooth),
                Mathf.Lerp(cam.rect.width, bigRect.width, Time.deltaTime * camSmooth),
                Mathf.Lerp(cam.rect.height, bigRect.height, Time.deltaTime * camSmooth));

            cam.transform.eulerAngles = new Vector3(Mathf.LerpAngle(cam.transform.eulerAngles.x, bigRot.x, Time.deltaTime * camRotSmooth),
                                                Mathf.LerpAngle(cam.transform.eulerAngles.y, bigRot.y, Time.deltaTime * camRotSmooth),
                                                Mathf.LerpAngle(cam.transform.eulerAngles.z, bigRot.z, Time.deltaTime * camRotSmooth));

            cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, 8, camTranSmooth), cam.transform.position.y, Mathf.Lerp(cam.transform.position.z, -0.2f, camTranSmooth));
            Time.timeScale = 0.6f;

        }
        else
        {
            legs.canTakeInput = true;

            mainCam.GetComponent<BlurOptimized>().enabled = false;

            cam.rect = new Rect(Mathf.Lerp(cam.rect.x, smallRect.x, Time.deltaTime * camSmooth),
                                 Mathf.Lerp(cam.rect.y, smallRect.y, Time.deltaTime * camSmooth),
                                 Mathf.Lerp(cam.rect.width, smallRect.width, Time.deltaTime * camSmooth),
                                 Mathf.Lerp(cam.rect.height, smallRect.height, Time.deltaTime * camSmooth));

            cam.transform.eulerAngles = new Vector3(Mathf.LerpAngle(cam.transform.eulerAngles.x, normRot.x, Time.deltaTime * camRotSmooth),
                                    Mathf.LerpAngle(cam.transform.eulerAngles.y, normRot.y, Time.deltaTime * camRotSmooth),
                                    Mathf.LerpAngle(cam.transform.eulerAngles.z, normRot.z, Time.deltaTime * camRotSmooth));

            cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, 5.78f, camTranSmooth), cam.transform.position.y, Mathf.Lerp(cam.transform.position.z, -5.88f, camTranSmooth));

            Time.timeScale = 1f;
        }
        for (int j = 0; j < babies.Count; j++)
        {

            if (babies[j].name == "" + i)
            {
                transform.GetChild(j).localScale = Vector3.Lerp(transform.GetChild(j).localScale, Vector3.one * 0.8f, smooth * t);
            }
            else
            {
                transform.GetChild(j).localScale = Vector3.Lerp(transform.GetChild(j).localScale, Vector3.one * 0.5f, smooth * t);
            }

            babies[j].transform.LookAt(cam.transform);
        }

        if (Input.GetAxisRaw("WeaponChange") == 0)
        {
            pressed = false;
        }


        if ((Input.GetKeyDown(KeyCode.E) || (Input.GetAxisRaw("WeaponChange") < -0.5f)) && !pressed)
        {
            RollUp(45);
        }
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetAxisRaw("WeaponChange") > 0.5f) && !pressed)
        {
            RollDown(-45);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, qTo, smooth * Time.deltaTime);

        


    }

    void iCheck()
    {
        if (i < 0)
        {
            i += 8;
        }
        if (i > 7)
        {
            i -= 8;
        }
    }
    public void RollUp(int rotAmount)
    {
        pressed = true;
        t = 0;
        desired += rotAmount;
        qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

        i -= rotAmount / 45;

        iCheck();

        if (i >= 0 && i <= 8)
        {
            beak.GetComponent<SpriteRenderer>().color = babies[i].GetComponent<SpriteRenderer>().color;
            ChangeColour();
        }
    }

    public void RollDown(int rotAmount)
    {
        pressed = true;
        t = 0;
        desired += rotAmount;
        qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

        i -= rotAmount / 45;

        iCheck();

        if (i >= 0 && i <= 8)
        {
            beak.GetComponent<SpriteRenderer>().color = babies[i].GetComponent<SpriteRenderer>().color;
            ChangeColour();
        }
    }

    void ChangeColour()
    {
        switch (i)
        {
            case 0:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Red";
                break;

            case 1:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Orange";
                break;

            case 2:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Yellow";
                break;

            case 3:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Green";
                break;

            case 4:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Blue";
                break;

            case 5:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Purple";
                break;

            case 6:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "Pink";
                break;

            case 7:
                beak.transform.GetChild(0).GetComponent<Shoot>().gunType = "White";
                break;

            default:
                print("bug!");
                break;

        }

    }

}

