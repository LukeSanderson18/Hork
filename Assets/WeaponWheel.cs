using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheel : MonoBehaviour
{
    public GameObject beak;
    public Camera cam;

    float startX;
     float desired;
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
        print(startX);
        foreach (Transform child in transform)
        {
            babies.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

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
            pressed = true;
            t = 0;
            desired += 45;
            qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

            if (i > 0)
                i--;
            else
                i = transform.childCount - 1;

            beak.GetComponent<SpriteRenderer>().color = babies[i].GetComponent<SpriteRenderer>().color;
            ChangeColour();
        }
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetAxisRaw("WeaponChange") > 0.5f) && !pressed)
        {
            pressed = true;
            t = 0;
            desired -= 45;
            qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

            if (i < transform.childCount - 1)
                i++;
            else
                i = 0;
            beak.GetComponent<SpriteRenderer>().color = babies[i].GetComponent<SpriteRenderer>().color;
            ChangeColour();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, qTo, smooth * Time.deltaTime);


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
