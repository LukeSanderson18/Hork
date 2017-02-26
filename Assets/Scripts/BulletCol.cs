﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCol : MonoBehaviour {

    public GameObject asplosion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Invoke("Wait", 0.01f);
    }
    void Wait()
    {
        Instantiate(asplosion, transform.position, transform.rotation);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
