using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class Legs : MonoBehaviour
{
    public Transform cameraFollowObject;
    public Vector2 gravityDirection = -Vector2.up;
    public Transform rotater;
    public float gravityUpScale = 3;
    public float gravityDownScale = 3;
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

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

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
               // rb.mass = 100;
                crouching = true;
            }
            else
            {
             //   rb.mass = 40;
                crouching = false;
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (foot1.GetComponent<groundCheck>().isGrounded || foot2.GetComponent<groundCheck>().isGrounded)
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
        RaycastHit2D downHit2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), gravityDirection, 10f, lm);
        if (downHit2.collider != null)
        {
            Quaternion quat = Quaternion.FromToRotation(Vector3.up, new Vector3(downHit2.normal.x, downHit2.normal.y, 0));

            //    print(downHit2.normal);
            gravityDirection = -downHit2.normal;// + (Vector2.one * 0.01f);
            if (walking)
                rotater.rotation =  Quaternion.Lerp(rotater.rotation, quat, Time.deltaTime * 10);
                //rotater.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(downHit2.normal.x, downHit2.normal.y, 0), Time.deltaTime * 4);
        }


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
               // leftTarget.transform.position = new Vector2(transform.GetChild(0).transform.GetChild(0).position.x,  transform.GetChild(0).transform.GetChild(0).position.y - leftDistance.y + targetOffset);
               // rightTarget.transform.position = new Vector2(transform.GetChild(0).transform.GetChild(1).position.x,  transform.GetChild(0).transform.GetChild(1).position.y - rightDistance.y + targetOffset);
            }

            //for uppage

            RaycastHit2D downHit = Physics2D.Raycast(transform.position, gravityDirection, 2.3f, lm);
            if (downHit.collider == null)
            {
               // print("not hitting.");
               rb.AddForce(-gravityDirection * gravityUpScale * Physics.gravity.y);
            }
            else
            {
                rb.AddForce(gravityDirection * gravityDownScale * Physics.gravity.y);
            }
            Debug.DrawRay(transform.position,gravityDirection,Color.red,3);

            cameraFollowObject.transform.position = cameraFollowObject.transform.parent.position;
        }
        else        //PLAYER IS ROLLING
        {
            gravityDirection = new Vector2(0, -1);
            rb.freezeRotation = false;

            /*RaycastHit2D downHit = Physics2D.Raycast(transform.position, gravityDirection, 2.3f, lm);
            
                rb.AddForce(-gravityDirection * gravityUpScale * Physics.gravity.y);
            
            rb.AddForce(Vector3.right * ((hor * Mathf.Abs(gravityDirection.y))) * rollSpeed  * Time.fixedDeltaTime);
            rb.AddForce(Vector3.up * ((ver * Mathf.Abs(gravityDirection.x))) * rollSpeed *  Time.fixedDeltaTime);
            */
            rb.AddForce(Vector2.right * hor * rollSpeed * Time.fixedDeltaTime);
            rb.AddTorque(rollSpeed * -hor * 0.01f);
            rb.AddForce(-gravityDirection * gravityUpScale * Physics.gravity.y);


            transform.GetChild(0).GetChild(0).localScale = Vector2.Lerp(transform.GetChild(0).GetChild(0).localScale, Vector2.zero, Time.fixedDeltaTime * 20);
            transform.GetChild(0).GetChild(1).localScale = Vector2.Lerp(transform.GetChild(0).GetChild(1).localScale, Vector2.zero, Time.fixedDeltaTime * 20);

            cameraFollowObject.transform.position = transform.position;

        }
    }
}
