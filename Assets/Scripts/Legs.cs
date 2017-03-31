using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
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
    float leftDistance;
    float rightDistance;

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
           /* else if (ver > 0.4f)
            {
               // rb.mass = 20;
                crouching = false;
            }
            * */

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
                        rb.velocity = rb.velocity + new Vector2(0, jumpHeight * 2f);
                    }
                    else
                    {
                        rb.velocity = rb.velocity + new Vector2(0, jumpHeight);
                    }
                }
            }
        }

        if (foot1.GetComponent<groundCheck>().isGrounded || foot2.GetComponent<groundCheck>().isGrounded)
        {
            isGrounded = true;
            footManager.gameObject.SetActive(true);
            square.transform.localScale = new Vector3(1.5f, 2, 1.5f);
        }
        else
        {
            isGrounded = false;
            footManager.gameObject.SetActive(false);
            square.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    void FixedUpdate()
    {
        if (walking)
        {
            transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, new Vector2(0.2f, 0.2f), Time.fixedDeltaTime * 20);
            transform.GetChild(1).localScale = Vector2.Lerp(transform.GetChild(1).localScale, new Vector2(0.2f, 0.2f), Time.fixedDeltaTime * 20);

            //move
            rb.AddForce(Vector3.right * hor * walkSpeed * Time.fixedDeltaTime);

            var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            rb.rotation = 0;
            rb.freezeRotation = true;

            //bounce == square
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, lm);
            if (hit.collider != null)
            {
                float distance = Mathf.Abs(hit.point.y - transform.position.y);
                square.transform.position = new Vector2(transform.position.x, (transform.position.y - distance) + 1.22f);
            }

            if (!isGrounded)
            {
                leftTarget.transform.position = new Vector2(transform.GetChild(0).position.x, transform.GetChild(0).position.y - leftDistance + targetOffset);
                rightTarget.transform.position = new Vector2(transform.GetChild(1).position.x, transform.GetChild(1).position.y - rightDistance + targetOffset);
            }

            /*
            if (Input.GetAxis("Crouch") > 0.2f && isGrounded)
            {
                print("called");
                rb.velocity = rb.velocity + new Vector2(0, jumpHeight * 0.5f);
            }
             * /      
            */

        }
        else
        {
            rb.freezeRotation = false;
            rb.AddTorque(-hor * rollSpeed);
            rb.AddForce(Vector3.right * hor * walkSpeed * 0.6f * Time.fixedDeltaTime);
            transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, Vector2.zero, Time.fixedDeltaTime * 20);
            transform.GetChild(1).localScale = Vector2.Lerp(transform.GetChild(1).localScale, Vector2.zero, Time.fixedDeltaTime * 20);
        }

        //RAYCASTS FOR LEGS HITTING FLOOR
        //FIDDLE WITH THIS SHIT
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y + 1.55f), -Vector2.up, Mathf.Infinity, lm);
        if (hitLeft.collider != null)
        {
            leftDistance = leftTarget.transform.position.y - hitLeft.point.y;
         
        }
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y + 1.55f), -Vector2.up, Mathf.Infinity, lm);
        if (hitRight.collider != null)
        {
            rightDistance = rightTarget.transform.position.y - hitRight.point.y;
        }

        //I KNOW THIS SHIT IS ALL THE SAME BUT IT LEGIT CRASHES IF I DONT SO... ???
        if (isGrounded)
        {
            if (leftDistance != rightDistance)
            {
                if (footManager.GOinFront == rightTarget)   //IF RIGHT FOOT IN FRONT
                {
                    leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance);
                    rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance);
                }
                else
                {
                    leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance);
                    rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance);
                }

            }
            else            //if floor is flat
            {
                leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance);
                rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance);
            }
        }
        else                //if not grounded
        {
            leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, leftTarget.transform.position.y - leftDistance);
            rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, rightTarget.transform.position.y - rightDistance);
        }


    }

    
}
