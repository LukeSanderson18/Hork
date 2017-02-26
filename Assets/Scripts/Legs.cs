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
            else if (ver > 0.4f)
            {
                rb.mass = 20;
                crouching = false;
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
    // Update is called once per frame
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

            //slopes for feet
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.GetChild(0).position, -Vector2.up, Mathf.Infinity, lm);
            if (hitLeft.collider != null)
            {
                leftDistance = Mathf.Abs(hitLeft.point.y - transform.position.y);
            }
            RaycastHit2D hitRight = Physics2D.Raycast(transform.GetChild(1).position, -Vector2.up, Mathf.Infinity, lm);
            if (hitRight.collider != null)
            {
                rightDistance = Mathf.Abs(hitRight.point.y - transform.position.y);
            }


            if (isGrounded)
            {
                if (rightDistance < leftDistance)           //IF RIGHT IS HIGHER
                {
                    if (footManager.GOinFront == rightTarget)   //IF RIGHT FOOT IN FRONT
                    {
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, transform.GetChild(0).position.y - rightDistance + targetOffset
                            - ((transform.GetChild(1).position.x - rightTarget.transform.position.x) / 4));//xdistance);
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, transform.GetChild(0).position.y - leftDistance + targetOffset
                            - ((transform.GetChild(0).position.x - leftTarget.transform.position.x) / 4));
                    }
                    else
                    {
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, transform.GetChild(0).position.y - rightDistance + targetOffset
                            - ((transform.GetChild(1).position.x - leftTarget.transform.position.x) / 4));//xdistance);
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, transform.GetChild(0).position.y - leftDistance + targetOffset
                            - ((transform.GetChild(0).position.x - rightTarget.transform.position.x) / 4));
                    }
                }
                else if (leftDistance < rightDistance)                             //IF LEFT IS HIGHER
                {
                    if (footManager.GOinFront == rightTarget)
                    {
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, transform.GetChild(0).position.y - rightDistance + targetOffset
                            + ((transform.GetChild(1).position.x - rightTarget.transform.position.x) / 4));//xdistance);
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, transform.GetChild(0).position.y - leftDistance + targetOffset
                            + ((transform.GetChild(0).position.x - leftTarget.transform.position.x) / 4));
                    }
                    else
                    {
                        leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, transform.GetChild(0).position.y - rightDistance + targetOffset
                                                + ((transform.GetChild(1).position.x - leftTarget.transform.position.x) / 4));
                        rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, transform.GetChild(0).position.y - leftDistance + targetOffset
                                                    + ((transform.GetChild(0).position.x - rightTarget.transform.position.x) / 4));
                    }
                }
                else
                {
                    leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, transform.GetChild(0).position.y - leftDistance + targetOffset);
                    rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, transform.GetChild(1).position.y - rightDistance + targetOffset);
                }
            }
            else
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


    }
}
