using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footTargets : MonoBehaviour {

    public GameObject player;
    Legs legs;
    public GameObject leftFoot;
    public GameObject rightFoot;
    public string inFront = "";
    public GameObject GOinFront;
    public bool isPlayer;

    public float offset = 1.2f;

    float refVel;
	// Use this for initialization
	void Start () {
        legs = player.GetComponent<Legs>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Abs(legs.gravityDirection.x) < Mathf.Abs(legs.gravityDirection.y))
        {
            if (rightFoot.transform.position.x > leftFoot.transform.position.x)
            {
                inFront = "right";
                GOinFront = rightFoot;
            }
            else
            {
                inFront = "left";
                GOinFront = leftFoot;
            }

            //move right

            if (inFront == "right" && player.transform.position.x > rightFoot.transform.position.x)
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset, leftFoot.transform.position.y);
            }

            else if (inFront == "left" && player.transform.position.x > leftFoot.transform.position.x)
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset, rightFoot.transform.position.y);
            }

            //move left
            else if (inFront == "right" && player.transform.position.x < leftFoot.transform.position.x)
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset, rightFoot.transform.position.y);
            }
            else if (inFront == "left" && player.transform.position.x < rightFoot.transform.position.x)
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset, leftFoot.transform.position.y);
            }

        }
        else        //ON WALLS
        {
            print("ON WALL");
            if (rightFoot.transform.position.y > leftFoot.transform.position.y)
            {
                inFront = "right";
                GOinFront = rightFoot;
            }
            else
            {
                inFront = "left";
                GOinFront = leftFoot;
            }

            //move right
            
            if (inFront == "right" && player.transform.position.y > rightFoot.transform.position.y)
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x, leftFoot.transform.position.y - offset);
            }

            else if (inFront == "left" && player.transform.position.y > leftFoot.transform.position.y)
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x, rightFoot.transform.position.y - offset);
            }

            //move left
            else if (inFront == "right" && player.transform.position.x < leftFoot.transform.position.x)
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x, rightFoot.transform.position.y + offset);
            }
            else if (inFront == "left" && player.transform.position.x < rightFoot.transform.position.x)
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x, leftFoot.transform.position.y + offset);
            }
             
        }
		
        
    }
}
