using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceRotate : MonoBehaviour {

    float hor;
    float ver;
    public bool aiming;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        hor = Input.GetAxis("RHor");
        ver = Input.GetAxis("RVer");

        if (hor != 0 || ver != 0)
        {
            if (transform.parent.parent.GetComponent<Legs>().walking)
            {
                float angle = Mathf.Atan2(hor, ver) * Mathf.Rad2Deg;
                Quaternion bane = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                transform.rotation = Quaternion.Lerp(transform.rotation, bane, Time.deltaTime * 10);
                aiming = true;
            }
        }
        else
        {
            aiming = false;
        }

    }
}
