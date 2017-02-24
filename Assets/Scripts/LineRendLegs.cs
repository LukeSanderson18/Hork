using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendLegs : MonoBehaviour {

    public Transform oneTran;
    public Transform twoTran;
    LineRenderer lr;
	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void LateUpdate () {

        lr.SetPosition(0, oneTran.position);
        lr.SetPosition(1, twoTran.position);


    }
}
