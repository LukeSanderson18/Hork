using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{

    public GameObject square;
     float hor;
     float ver;
    bool isGrounded;
    public float walkSpeed = 5;
    public float jumpHeight = 6000;

    public GameObject eyeWhite;
    public GameObject eyeBlack;
    public GameObject beak;

    public GameObject leftTarget;
    public GameObject rightTarget;

    Vector2 refVelocity1;
    Vector2 refVelocity2;
    Vector2 refVelocity3;
    float refV;


    Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "s")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "s")
        {
            isGrounded = false;
        }
    }
    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        rb.AddForce(transform.right * hor * walkSpeed * Time.deltaTime);

        //bounce
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Floor"));
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            square.transform.position = new Vector2(transform.position.x, (transform.position.y - distance)+1.22f);

        }

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.GetChild(0).transform.position, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Floor"));
        if (hitLeft.collider != null)
        {
           // leftTarget.transform.position = new Vector2(transform.GetChild(0).transform.position.x, hitLeft.point.y);
        }
        RaycastHit2D hitRight = Physics2D.Raycast(transform.GetChild(1).transform.position, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Floor"));
        if (hitRight.collider != null)
        {
            //rightTarget.transform.position = new Vector2(transform.GetChild(1).transform.position.x, hitRight.point.y);
        }
        //jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight);
        }

        //facial features -- fix this!!
        if(rb.velocity.x > .02f)
        {
            eyeWhite.transform.localPosition = Vector2.SmoothDamp(eyeWhite.transform.localPosition, new Vector2(0.17f, eyeWhite.transform.localPosition.y), ref refVelocity1, 0.2f,50, Time.deltaTime);
            eyeBlack.transform.localPosition = Vector2.SmoothDamp(eyeBlack.transform.localPosition, new Vector2(0.23f, eyeBlack.transform.localPosition.y), ref refVelocity3, 0.2f, 50, Time.deltaTime);
            beak.transform.localPosition = Vector2.SmoothDamp(beak.transform.localPosition, new Vector2(0.451f, beak.transform.localPosition.y), ref refVelocity2, 0.2f, 50, Time.deltaTime);
            beak.transform.localScale = new Vector2(beak.transform.localScale.x, Mathf.SmoothDamp(beak.transform.localScale.y, 0.806f, ref refV, Time.deltaTime*10));
        }
        else if (rb.velocity.x < -.02f)
        {
            eyeWhite.transform.localPosition = Vector2.SmoothDamp(eyeWhite.transform.localPosition, new Vector2(-0.17f, eyeWhite.transform.localPosition.y), ref refVelocity1, 0.2f, 50, Time.deltaTime);
            eyeBlack.transform.localPosition = Vector2.SmoothDamp(eyeBlack.transform.localPosition, new Vector2(-0.23f, eyeBlack.transform.localPosition.y), ref refVelocity3, 0.2f, 50, Time.deltaTime);
            beak.transform.localPosition = Vector2.SmoothDamp(beak.transform.localPosition, new Vector2(-0.451f, beak.transform.localPosition.y), ref refVelocity2, 0.2f, 50, Time.deltaTime);
            beak.transform.localScale = new Vector2(beak.transform.localScale.x, Mathf.SmoothDamp(beak.transform.localScale.y, -0.806f, ref refV, Time.deltaTime*10));

        }
    }
}
