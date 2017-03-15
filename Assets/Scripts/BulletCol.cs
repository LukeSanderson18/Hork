using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCol : MonoBehaviour {

    public ParticleSystem asplosion;
    public ParticleSystem bulletHit;
    public Color bulletHitColor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Invoke("Wait", 0.01f);
    }
    void Wait()
    {
        ParticleSystem ps1 = Instantiate(asplosion, transform.position, transform.rotation);

        ParticleSystem pS = Instantiate(bulletHit, transform.position, Quaternion.identity);
        //
             ParticleSystem.MinMaxGradient randomColorGradient = new ParticleSystem.MinMaxGradient();
     randomColorGradient.mode = ParticleSystemGradientMode.TwoColors;
     randomColorGradient.colorMin = Color.white;
     randomColorGradient.colorMax = bulletHitColor;
     
     //Assign the gradient back to the ParticleSystem
     ParticleSystem.MainModule mainModule = pS.main;
     ParticleSystem.MainModule mainModule2 = ps1.main;

     mainModule.startColor = randomColorGradient;
     mainModule2.startColor = randomColorGradient;
     //
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
