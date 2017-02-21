using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footTargets : MonoBehaviour {

    public GameObject player;
    public GameObject leftFoot;
    public GameObject rightFoot;
    public string inFront = "";

    float refVel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(rightFoot.transform.position.x > leftFoot.transform.position.x)
        {
            inFront = "right";
        }
        else
        {
            inFront = "left";
        }
		
        //move right
        if (inFront == "right" && player.transform.position.x-.1f > rightFoot.transform.position.x)
        {
            print("1");
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x+1.2f, leftFoot.transform.position.y);
        }
        
        else if (inFront == "left" && player.transform.position.x > leftFoot.transform.position.x)
        {
            print("2");
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + 1.2f, rightFoot.transform.position.y);
        }

        //move left
        else if (inFront == "right" && player.transform.position.x < leftFoot.transform.position.x)
        {
            print("3");
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - 1.2f, rightFoot.transform.position.y);
        }
        else if (inFront == "left" && player.transform.position.x < rightFoot.transform.position.x)
        {
            print("4");
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - 1.2f, leftFoot.transform.position.y);
        }
    }
}
