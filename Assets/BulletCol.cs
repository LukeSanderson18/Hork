using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCol : MonoBehaviour {

    public GameObject asplosion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(asplosion, transform.position, transform.rotation);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
