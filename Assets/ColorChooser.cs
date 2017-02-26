using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChooser : MonoBehaviour
{

    float hor;
    float ver;
    public bool aiming;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        hor = -Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Weapon1"))
        {
            transform.eulerAngles = new Vector3(-90, 0, 0);
        }
        if ((hor != 0 || ver != 0) && Input.GetButton("Weapon1"))
        {

            float angle = Mathf.Atan2(hor, ver) * Mathf.Rad2Deg;
            Quaternion bane = Quaternion.Euler(new Vector3(-angle, 0, 0));
            transform.rotation = Quaternion.Lerp(transform.rotation, bane, Time.deltaTime * 30);
            aiming = true;

        }
        else
        {
            aiming = false;
        }

        float angley = (Mathf.Atan2(transform.forward.z, transform.forward.y) * Mathf.Rad2Deg);
        float newangley = ((360 + angley));

        float finalAngle = (((newangley + angley) / 2) / 45);
        float finalInt = Mathf.Round(finalAngle);

        if (Input.GetButtonUp("Weapon1"))
        {
            //4 is left!!

            if (finalInt == 0 || finalInt == 8)
            {
                transform.parent.GetComponent<WeaponWheel>().RollUp(180);
            }
            if (finalInt == 1)
            {
                transform.parent.GetComponent<WeaponWheel>().RollUp(135);
            }
            if (finalInt == 2)
            {
                transform.parent.GetComponent<WeaponWheel>().RollUp(90);
            }
            if (finalInt == 3)
            {
                transform.parent.GetComponent<WeaponWheel>().RollUp(45);
            }
            if (finalInt == 5)
            {
                transform.parent.GetComponent<WeaponWheel>().RollDown(-45);
            }
            if (finalInt == 6)
            {
                transform.parent.GetComponent<WeaponWheel>().RollDown(-90);
            }
            if (finalInt == 7)
            {
                transform.parent.GetComponent<WeaponWheel>().RollDown(-135);
            }
        }


    }
}
