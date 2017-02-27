using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    Vector2 startPos;
    bool turnedOn = false;
    public Transform door;

    void Start()
    {
        startPos = door.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BulletDamage>())
        {
            turnedOn = !turnedOn;
        }
    }

    void Update()
    {
        if (turnedOn)
        {
            door.position = Vector2.Lerp(door.position, new Vector2(door.position.x, 25), Time.deltaTime * 8);

        }
        else
        {
            door.position = Vector2.Lerp(door.position, new Vector2(door.position.x, 15), Time.deltaTime * 8);

        }
    }
}
