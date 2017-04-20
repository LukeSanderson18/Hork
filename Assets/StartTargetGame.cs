using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTargetGame : MonoBehaviour {

    public TargetGame targetGameObj;
    public bool running;
    public int numberOfTargets = 0;
    float Timer;
    public Text text;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BulletDamage>())
        {
            print("hit me!");
            if(!running)
            {
                running = true;
                targetGameObj.CreateTarget();
                Timer = 0;
            }
        }
    }

    private void Update()
    {
        if (running)
        {
            Timer += Time.deltaTime;
            text.text = Timer.ToString("F2");
        }
    }
}
