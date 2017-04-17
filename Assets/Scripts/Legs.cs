using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    public Vector2 gravityDirection = -Vector2.up;
    public Transform rotater;
    public float gravityScale = 3;
    public bool canTakeInput;
    public bool walking = true;
    bool crouching;
    public GameObject square;
    public float hor;
    public float ver;
    public bool isGrounded;
    public float walkSpeed = 5;
    public float jumpHeight = 6000;
    public float rollSpeed = 20;

    public GameObject eyeWhite;
    public GameObject eyeBlack;
    public GameObject beak;

    public GameObject foot1;
    public GameObject foot2;

    public GameObject leftTarget;
    public GameObject rightTarget;
    public float targetOffset;
    Vector2 leftDistance;
    Vector2 rightDistance;

    Vector2 refVelocity1;
    Vector2 refVelocity2;
    Vector2 refVelocity3;
    float refV;
    public FaceRotate fr;

    public LayerMask lm;

    public footTargets footManager;

    Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
     //   print(gravityDirection);
        if (canTakeInput)
        {
            hor = Input.GetAxis("Horizontal");
            ver = Input.GetAxis("Vertical");

            if (Input.GetAxis("Crouch") > 0.2f)
            {
                walking = false;
                square.SetActive(false);
            }
            else
            {
                walking = true;
                square.SetActive(true);
            }


            if (ver < -0.4f)
            {
                rb.mass = 100;
                crouching = true;
            }
            else
            {
                rb.mass = 40;
                crouching = false;
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (foot1.GetComponent<groundCheck>().isGrounded && foot2.GetComponent<groundCheck>().isGrounded)
                {
                    if (crouching)
                    {
                        rb.velocity = rb.velocity + (-gravityDirection * jumpHeight * 2);//new Vector2(0, jumpHeight);
                    }
                    else
                    {
                        rb.velocity = rb.velocity + (-gravityDirection * jumpHeight);//new Vector2(0, jumpHeight);
                    }
                }
            }
        }

        if (foot1.GetComponent<groundCheck>().isGrounded || foot2.GetComponent<groundCheck>().isGrounded)
        {
            isGrounded = true;
           // footManager.gameObject.SetActive(true);
            square.transform.localScale = new Vector3(1.5f, 2, 1.5f);
        }
        else
        {
            isGrounded = false;
           // footManager.gameObject.SetActive(false);
            square.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    void FixedUpdate()
    {
        if (walking)
        {
            transform.GetChild(0).transform.GetChild(0).localScale = Vector2.Lerp( transform.GetChild(0).transform.GetChild(0).localScale, new Vector2(0.2f, 0.2f), Time.fixedDeltaTime * 20);
            transform.GetChild(0). transform.GetChild(1).localScale = Vector2.Lerp( transform.GetChild(0).transform.GetChild(1).localScale, new Vector2(0.2f, 0.2f), Time.fixedDeltaTime * 20);

            //move
            rb.AddForce(Vector3.right * ((hor * Mathf.Abs(gravityDirection.y))) * walkSpeed * Time.fixedDeltaTime);
            rb.AddForce(Vector3.up * ((ver * Mathf.Abs(gravityDirection.x))) * walkSpeed * Time.fixedDeltaTime);

           // var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            rb.rotation = 0;
            rb.freezeRotation = true;

            if (!isGrounded)
            {
              //  leftTarget.transform.position = new Vector2(transform.GetChild(0).transform.GetChild(0).position.x,  transform.GetChild(0).transform.GetChild(0).position.y - leftDistance.y + targetOffset);
              //  rightTarget.transform.position = new Vector2(transform.GetChild(0).transform.GetChild(1).position.x,  transform.GetChild(0).transform.GetChild(1).position.y - rightDistance.y + targetOffset);
            }

            rb.AddForce(gravityDirection * gravityScale * Physics.gravity.y);

            RaycastHit2D downHit = Physics2D.Raycast(transform.position, gravityDirection, 2.3f, lm);
            if (downHit.collider != null)
            {
                rb.AddForce(-gravityDirection * gravityScale * Physics.gravity.y * 1.5f);
            }

            RaycastHit2D downHit2 = Physics2D.Raycast(new Vector2(transform.position.x+0.0f,transform.position.y), gravityDirection, 2.3f, lm);
            if (downHit2.collider != null)
            {
            //    print(downHit2.normal);
                gravityDirection = -downHit2.normal;
                rotater.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(downHit2.normal.x,downHit2.normal.y,0));
                //rb.AddForce(-gravityDirection * gravityScale * Physics.gravity.y * 1.5f);
            }
            else
            {
            //    gravityDirection = new Vector2(0, -1);
            }



        }
        else        //PLAYER IS ROLLING
        {
            gravityDirection = -Vector2.up;
            rb.freezeRotation = false;
            rb.AddTorque(-hor * rollSpeed);
            rb.AddForce(Vector3.right * hor * walkSpeed * 0.6f * Time.fixedDeltaTime);
            transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, Vector2.zero, Time.fixedDeltaTime * 20);
            transform.GetChild(1).localScale = Vector2.Lerp(transform.GetChild(1).localScale, Vector2.zero, Time.fixedDeltaTime * 20);
        }

       // print(gravityDirection);


        if(false)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(leftTarget.transform.position.x - gravityDirection.x, leftTarget.transform.position.y + (1.55f * -gravityDirection.y)), gravityDirection, Mathf.Infinity, lm);
            Debug.DrawRay(new Vector2(leftTarget.transform.position.x - gravityDirection.x, leftTarget.transform.position.y + (0.55f * -gravityDirection.y)), gravityDirection, Color.red, 0.1f);
            if (hitLeft.collider != null)
            {
                leftDistance = (Vector2)leftTarget.transform.position - hitLeft.point;
            }
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(rightTarget.transform.position.x - gravityDirection.x, rightTarget.transform.position.y + (1.55f * -gravityDirection.y)), gravityDirection, Mathf.Infinity, lm);
            if (hitRight.collider != null)
            {
                rightDistance = (Vector2)rightTarget.transform.position - hitRight.point;
            }

            //I KNOW THIS SHIT IS ALL THE SAME BUT IT LEGIT CRASHES IF I DONT SO... ???
            if (isGrounded)
            {
                if (leftDistance != rightDistance)
                {
                    if (footManager.GOinFront == rightTarget)   //IF RIGHT FOOT IN FRONT
                    {
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance.y);
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance.y);
                    }
                    else
                    {
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance.y);
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance.y);
                    }
                }
                else            //if floor is flat
                {
                    //   print("distance same!");
                    leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance.y);
                    rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance.y);
                }
            }
            else                //if not grounded
            {
                leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance.y);
                rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance.y);
            }
        }
        //Debug.Log("FHAJSFH");
        //RAYCASTS FOR LEGS HITTING FLOOR
        //FIDDLE WITH THIS SHIT

        //

        ///
        //
        //
        //TO DO
        //
        //CONVERT LEFT DISTNACE AND RIGHT DISTANCE INTO VECTOR2S
        //
        //THATS IT

        //THESE HAVE NOTHING TO DO WITH ACTUALLY MOVING THE PLAYER, JUST FOR LEG A E S T H E T I C

        /*
        
        

         
    /**/

    }



}
