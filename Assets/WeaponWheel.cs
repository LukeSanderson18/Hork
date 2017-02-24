using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWheel : MonoBehaviour
{

    public Camera cam;

    float startX;
    public float desired;
    public float smooth = 0.3f;
    private Quaternion qTo = Quaternion.identity;

    public int i;

    // Use this for initialization
    void Start()
    {
        startX = transform.eulerAngles.x;
        desired = startX;
        print(startX);
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Transform child in transform)
        {
            child.transform.LookAt(cam.transform);
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            desired += 45;
            qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

            if (i > 0)
                i--;
            else
                i = transform.childCount - 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            desired -= 45;
            qTo = Quaternion.Euler(desired, 0.0f, 0.0f);

            if (i < transform.childCount - 1)
                i++;
            else
                i = 0;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, qTo, smooth * Time.deltaTime);
        //Quaternion targetRotation =   Quaternion.Euler(desired,transform.eulerAngles.y, transform.eulerAngles.z);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smooth);
        //transform.RotateAround(transform.position, transform.right, Time.deltaTime * 90f);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smooth);

    }

}
