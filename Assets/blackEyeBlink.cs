using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blackEyeBlink : MonoBehaviour {

    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        InvokeRepeating("Blink", 1,0.8f);
	}
	
	void Blink()
    {
        int randInt = Random.Range(0,9);
        print(randInt);

        if (randInt <= 1)
        {
            anim.Play("blackEyeBlink");
        }
        if (randInt == 2)
        {
            anim.Play("blackEyeTwoBlink");
        }
    }
}
