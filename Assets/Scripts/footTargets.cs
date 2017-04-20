using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footTargets : MonoBehaviour
{
    public groundCheck leftGroundCheck;
    public groundCheck rightGroundCheck;
    public LayerMask lm;
    public GameObject player;
    public Transform rotator;
    Legs legs;
    public GameObject leftFoot;
    public GameObject rightFoot;
    public Transform leftSocket;
    public Transform rightSocket;

    public Transform leftFloorPoint;
    public Transform rightFloorPoint;
    public string inFront = "";
    public GameObject GOinFront;
    public GameObject GOinBack;
    public bool isPlayer;

    public float offset = 1.2f;

    public float disty = 1f;


    float readyTimer;
    public bool touching;

    float refVel;
    // Use this for initialization
    void Start()
    {
        legs = player.GetComponent<Legs>();
    }

    // Update is called once per frame

    /*
void Update()
{


    //move right

    if (false)
    {
        print(player.transform.position.x * -legs.gravityDirection.y + ", " + rightFoot.transform.position.x * -legs.gravityDirection.y);

        if (inFront == "right" && player.transform.position.x > rightFoot.transform.position.x)
        {
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * legs.gravityDirection.x);
            inFront = "left";
            GOinFront = leftFoot;
        }

        else if (inFront == "left" && player.transform.position.x > leftFoot.transform.position.x)
        {
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * legs.gravityDirection.x);
            inFront = "right";
            GOinFront = rightFoot;
        }


        //move left
        else if (inFront == "right" && player.transform.position.x < leftFoot.transform.position.x)
        {
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * -legs.gravityDirection.x);
            inFront = "right";
            GOinFront = rightFoot;
        }
        else if (inFront == "left" && player.transform.position.x < rightFoot.transform.position.x)
        {
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * -legs.gravityDirection.x);
            inFront = "left";
            GOinFront = leftFoot;
        }


    }



    RaycastHit2D hitLeft = Physics2D.Raycast(leftFoot.transform.position, -legs.gravityDirection, Mathf.Infinity, lm);
    RaycastHit2D hitRight = Physics2D.Raycast(rightFoot.transform.position, -legs.gravityDirection, Mathf.Infinity, lm);

    //RaycastHit2D leftPointToPlayer = Physics2D.Raycast(leftFoot.transform.position, (player.transform.position - leftFoot.transform.position), Mathf.Infinity, lm);
    //RaycastHit2D rightPointToPlayer = Physics2D.Raycast(rightFoot.transform.position, (player.transform.position - rightFoot.transform.position), Mathf.Infinity, lm);

    // print(player.transform.position - leftFoot.transform.position);

    if (true)
    {

        float asdfRight = Mathf.Abs(player.transform.position.y - (player.transform.position.y - rightFoot.transform.position.y));
        float asdfLeft = Mathf.Abs(player.transform.position.y - (player.transform.position.y - leftFoot.transform.position.y));
        float xFloat = rightFoot.transform.position.x * legs.gravityDirection.x;
        //  print(("PT: " + asdfRight + ", has to be greater than: " + xFloat));
        //print(rightFoot .transform.position.x * -legs.gravityDirection.y);

        // if (Mathf.Abs(legs.gravityDirection.x) < 0.3f)

        //print("HHHHHHHHHHH" + player.transform.position.x + " , " + leftFoot.transform.position.x * -legs.gravityDirection.y);
        if (legs.gravityDirection.y == -1)
        {
            if (player.transform.position.x > rightFoot.transform.position.x * -legs.gravityDirection.y)// && asdfRight  > rightFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "right")
                {
                    leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * legs.gravityDirection.x);
                    inFront = "left";
                    //Ting(1);
                }
            }
            if (player.transform.position.x > leftFoot.transform.position.x * -legs.gravityDirection.y)// && asdfLeft > leftFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "left")
                {
                    rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * legs.gravityDirection.x);
                    inFront = "right";
                    //Ting(1);
                }
            }
            if (player.transform.position.x < rightFoot.transform.position.x * -legs.gravityDirection.y)// && asdfRight < rightFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "left")
                {
                    leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset * -legs.gravityDirection.y, rightFoot.transform.position.y - offset * legs.gravityDirection.x);
                    inFront = "right";
                    //Ting(1);
                }
            }
            if (player.transform.position.x < leftFoot.transform.position.x * -legs.gravityDirection.y)// && asdfLeft  < leftFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "right")
                {
                    rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset * -legs.gravityDirection.y, leftFoot.transform.position.y - offset * legs.gravityDirection.x);
                    inFront = "left";
                    //Ting(1);
                }
            }
        }
        //
        //
        //wall test
        //
        //
        else if (legs.gravityDirection.x == 1)
        {
            if (player.transform.position.y > rightFoot.transform.position.y * legs.gravityDirection.x)// && asdfRight  > rightFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "right")
                {
                    leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * legs.gravityDirection.x);
                    inFront = "left";
                    //Ting(1);
                }
            }
            if (player.transform.position.y > leftFoot.transform.position.y * legs.gravityDirection.x)// && asdfLeft > leftFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "left")
                {
                    rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * legs.gravityDirection.x);
                    inFront = "right";
                    //Ting(1);
                }
            }

            if (player.transform.position.y < rightFoot.transform.position.y * legs.gravityDirection.x)// && asdfRight < rightFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "left")
                {
                    leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset * -legs.gravityDirection.y, rightFoot.transform.position.y - offset * legs.gravityDirection.x);
                    inFront = "right";
                    //Ting(1);
                }
            }


            if (player.transform.position.y < leftFoot.transform.position.y * legs.gravityDirection.x)// && asdfLeft  < leftFoot.transform.position.x * -legs.gravityDirection.x)
            {
                if (inFront == "right")
                {
                    rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset * -legs.gravityDirection.y, leftFoot.transform.position.y - offset * legs.gravityDirection.x);
                    inFront = "left";
                    //Ting(1);
                }
            }
        }
    }
    //else

    if (false)
    {
        if (inFront == "left")
        {
            GOinFront = leftFoot;
            GOinBack = rightFoot;
        }
        else
        {
            GOinFront = rightFoot;
            GOinBack = leftFoot;
        }

        float a = player.transform.position.y - GOinFront.transform.position.y;
        float b = player.transform.position.x - GOinFront.transform.position.x;

        float c = player.transform.position.y - GOinBack.transform.position.y;
        float d = player.transform.position.x - GOinBack.transform.position.x;

        float angle1 = Mathf.Tan(a / b);

        float angle2 = Mathf.Tan(c / d);

        //print(Mathf.Tan(legs.gravityDirection.y / legs.gravityDirection.x));
        if (Mathf.Tan(legs.gravityDirection.y / legs.gravityDirection.x) - angle1 < 0.01)
        {
            if (GOinFront == rightFoot)
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * legs.gravityDirection.x);
                inFront = "left";
                GOinFront = leftFoot;
            }
            else
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * legs.gravityDirection.x);
                inFront = "right";
                GOinFront = rightFoot;

            }
        }
        if (Mathf.Tan(legs.gravityDirection.y / legs.gravityDirection.x) - angle2 > 0.01)
        {
            if (GOinFront == rightFoot)
            {
                rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset * -legs.gravityDirection.y, leftFoot.transform.position.y - offset * legs.gravityDirection.x);
                inFront = "left";
                GOinFront = leftFoot;
            }
            else
            {
                leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset * -legs.gravityDirection.y, rightFoot.transform.position.y - offset * legs.gravityDirection.x);

                inFront = "right";
                GOinFront = rightFoot;

            }
        }
    }
}

/*else                //on wall
{
    print("ON WALL");
    if (player.transform.position.y > rightFoot.transform.position.x * legs.gravityDirection.x)
    {
        if (inFront == "right")
        {
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x + offset * -legs.gravityDirection.y, rightFoot.transform.position.y + offset * legs.gravityDirection.x);
            inFront = "left";
            print("1");
            //Ting(1);
        }
    }
    if (player.transform.position.y  > leftFoot.transform.position.x * legs.gravityDirection.x)
    {
        if (inFront == "left")
        {
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x + offset * -legs.gravityDirection.y, leftFoot.transform.position.y + offset * legs.gravityDirection.x);
            inFront = "right";
            print("2");

            //Ting(1);
        }
    }
    if (player.transform.position.y  < rightFoot.transform.position.x * legs.gravityDirection.x)
    {
        if (inFront == "left")
        {
            leftFoot.transform.position = new Vector2(rightFoot.transform.position.x - offset * -legs.gravityDirection.y, rightFoot.transform.position.y - offset * legs.gravityDirection.x);
            inFront = "right";
            print("3");

            //Ting(1);
        }
    }
    if (player.transform.position.y  < leftFoot.transform.position.x * legs.gravityDirection.x)
    {
        if (inFront == "right")
        {
            rightFoot.transform.position = new Vector2(leftFoot.transform.position.x - offset * -legs.gravityDirection.y, leftFoot.transform.position.y - offset * legs.gravityDirection.x);
            inFront = "left";
            print("1");

            //Ting(1);
        }
    }
}
*/
    // else
    // {
    //     Ting(0);
    // }

    // if (player.transform.position.x > rightFoot.transform.position.x * -legs.gravityDirection.y || player.transform.position.y > rightFoot.transform.position.x * legs.gravityDirection.x)
    // {
    //     Ting(1);
    // }

    //print(touching())
    //  Debug.DrawRay(leftFoot.transform.position, -legs.gravityDirection, Color.yellow, 2);


    private void Update()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(leftSocket.position.x + (legs.gravityDirection.x),leftSocket.position.y + (legs.gravityDirection.y)), legs.gravityDirection, 111, lm);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(rightSocket.position.x + (legs.gravityDirection.x), rightSocket.position.y + (legs.gravityDirection.y)), legs.gravityDirection, 111, lm);

        float extraDistance = Mathf.Abs(legs.gravityDirection.x) + Mathf.Abs(legs.gravityDirection.y);

        if (hitLeft.collider != null)
        {
            print("hitting summat");
            leftFloorPoint.position = hitLeft.point;
        }
        if (hitRight.collider != null)
        {
            rightFloorPoint.position = hitRight.point;
        }


        if (Vector2.Distance(player.transform.position, leftFoot.transform.position) > disty + extraDistance)
        {
            if(Vector2.Distance(leftFoot.transform.position,leftFloorPoint.position)>Vector2.Distance(leftFoot.transform.position,rightFloorPoint.position))
            {
                leftFoot.transform.position = leftFloorPoint.position;
            }
            else
            {
                leftFoot.transform.position = rightFloorPoint.position;

            }
        }

        if (Vector2.Distance(player.transform.position, rightFoot.transform.position) > disty + extraDistance)
        {
            if (Vector2.Distance(rightFoot.transform.position, rightFloorPoint.position) > Vector2.Distance(rightFoot.transform.position, leftFloorPoint.position))
            {
                rightFoot.transform.position = rightFloorPoint.position;
            }
            else
            {
                rightFoot.transform.position = leftFloorPoint.position;

            }
        }

        //if feet too close together... dirty.
        if (Vector2.Distance(leftFoot.transform.position,rightFoot.transform.position) < 0.7f)
        {
            print("too close!");
            if(Vector2.Distance(leftFoot.transform.position,leftFloorPoint.position) < Vector2.Distance(rightFoot.transform.position,leftFloorPoint.position))
            {
                rightFoot.transform.position = rightFloorPoint.transform.position;
            }
            if (Vector2.Distance(leftFoot.transform.position, leftFloorPoint.position) > Vector2.Distance(rightFoot.transform.position, leftFloorPoint.position))
            {
                leftFoot.transform.position = leftFloorPoint.transform.position;
            }
            if(Vector2.Distance(rightFoot.transform.position,leftFloorPoint.position) > Vector2.Distance(leftFoot.transform.position,leftFloorPoint.position))
            {
                rightFoot.transform.position = leftFloorPoint.transform.position;
            }
            else
            {
                leftFoot.transform.position = rightFloorPoint.transform.position;
            }
        }
    }



  
}
