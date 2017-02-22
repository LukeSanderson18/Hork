using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    public bool walking = true;
    bool crouching;
    public GameObject square;
    float hor;
    float ver;
    bool isGrounded;
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

    Vector2 refVelocity1;
    Vector2 refVelocity2;
    Vector2 refVelocity3;
    float refV;
    public FaceRotate fr;

    Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
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

        if(ver < -0.4f)
        {
            rb.mass = 100;
            crouching = true;
        }
        else
        {
            rb.mass = 40;
            crouching = false;
        }

        //jump
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
    // Update is called once per frame
    void FixedUpdate()
    {
        if (walking)
        {
            transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, new Vector2(0.2f, 0.2f), Time.deltaTime * 20);
            transform.GetChild(1).localScale = Vector2.Lerp(transform.GetChild(1).localScale, new Vector2(0.2f, 0.2f), Time.deltaTime * 20);

            //move
            rb.AddForce(Vector3.right * hor * walkSpeed * Time.deltaTime);

            var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            rb.rotation = 0;
            rb.freezeRotation = true;

            //bounce
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Floor"));
            if (hit.collider != null)
            {
                float distance = Mathf.Abs(hit.point.y - transform.position.y);
                square.transform.position = new Vector2(transform.position.x, (transform.position.y - distance) + 1.22f);
                leftTarget.transform.position = new Vector2(leftTarget.transform.position.x, square.transform.position.y - 1.2f);
                rightTarget.transform.position = new Vector2(rightTarget.transform.position.x, square.transform.position.y - 1.2f);

            }

            

            //facial features -- fix this!!

            /*
            if (rb.velocity.x >= 0f)
            {
                if (!fr.aiming)
                {
                    eyeWhite.transform.localPosition = Vector2.SmoothDamp(eyeWhite.transform.localPosition, new Vector2(0.17f, eyeWhite.transform.localPosition.y), ref refVelocity1, 0.2f, 50, Time.deltaTime);
                    eyeBlack.transform.localPosition = Vector2.SmoothDamp(eyeBlack.transform.localPosition, new Vector2(0.23f, eyeBlack.transform.localPosition.y), ref refVelocity3, 0.2f, 50, Time.deltaTime);
                    beak.transform.localPosition = Vector2.SmoothDamp(beak.transform.localPosition, new Vector2(0.451f, beak.transform.localPosition.y), ref refVelocity2, 0.2f, 50, Time.deltaTime);

                    beak.transform.localScale = new Vector2(0.806f, Mathf.SmoothDamp(beak.transform.localScale.y, 0.806f, ref refV, Time.deltaTime * 10));

                }
            }
            else if (rb.velocity.x < -.02f)
            {
                if (!fr.aiming)
                {
                    eyeWhite.transform.localPosition = Vector2.SmoothDamp(eyeWhite.transform.localPosition, new Vector2(-0.17f, eyeWhite.transform.localPosition.y), ref refVelocity1, 0.2f, 50, Time.deltaTime);
                    eyeBlack.transform.localPosition = Vector2.SmoothDamp(eyeBlack.transform.localPosition, new Vector2(-0.23f, eyeBlack.transform.localPosition.y), ref refVelocity3, 0.2f, 50, Time.deltaTime);
                    beak.transform.localPosition = Vector2.SmoothDamp(beak.transform.localPosition, new Vector2(-0.451f, beak.transform.localPosition.y), ref refVelocity2, 0.2f, 50, Time.deltaTime);

                    beak.transform.localScale = new Vector2(-0.806f, Mathf.SmoothDamp(beak.transform.localScale.y, -0.806f, ref refV, Time.deltaTime * 10));

                }

            }
            */

        }
        else
        {
            rb.freezeRotation = false;
            rb.AddTorque(-hor * rollSpeed);
            transform.GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).localScale, Vector2.zero, Time.deltaTime * 20);
            transform.GetChild(1).localScale = Vector2.Lerp(transform.GetChild(1).localScale, Vector2.zero, Time.deltaTime * 20);
        }

        if (Input.GetAxis("Crouch") > 0.2f && walking)
        {
            rb.velocity = rb.velocity + new Vector2(0, jumpHeight*0.5f);
        }
    }
}
