using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public int health;
    public Image[] hearts;

    void Heart()
    {
        for (int i = 0; i < health; i++)
        {
            hearts[i].color = Color.red;
            hearts[i].transform.localScale = Vector3.one;
        }
        for (int i = health; i < 3; i++)
        {
            hearts[i].color = Color.black;
            hearts[i].transform.localScale = Vector3.one * 0.6f;

        }
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (health < 3)
            {
                health++;
                Heart();
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (health > 0)
            {
                health--;
                Heart();
            }
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].color = Color.Lerp(hearts[i].color, Color.clear, Time.deltaTime);
        }

        transform.rotation = Quaternion.identity;

    }
}
