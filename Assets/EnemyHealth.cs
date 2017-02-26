using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public bool child;
    public Material whiteMaterial;
    float floaty = 0f;
    public int health;
    int maxHealth;
    public float lerpSpeed = 10f;
    public Image img;
    bool canGetOriginalColor = true;
    bool alive = true;

    void Start()
    {
        img = GetComponentInChildren<Image>();
        floaty = health;
        maxHealth = health;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("hit.");
        if (collision.gameObject.GetComponent<BulletDamage>())
        {
            print("HIT ME!");

            health -= collision.gameObject.GetComponent<BulletDamage>().bulletDam;
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            StartCoroutine(FlashSprites(sprites, 1, 0.09f));
        }
    }

    IEnumerator FlashSprites(SpriteRenderer[] sprites, int numTimes, float delay, bool disable = false)
    {
        Material normalMaterial = null;

        // number of times to loop
        for (int i = 0; i < sprites.Length; i++)
        {
            normalMaterial = sprites[i].material;
        }

        for (int loop = 0; loop < numTimes; loop++)
        {
            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].material = whiteMaterial;
                // sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0.5f);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);

            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].material = normalMaterial;
                // sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
    }

    void Update()
    {
        floaty = Mathf.Lerp(floaty, health, Time.deltaTime * lerpSpeed);
        img.fillAmount = floaty / maxHealth;

        if (health <= 0 && alive)
        {
            if (child)
            {
                transform.root.GetComponent<Shake>().shakeDuration = 0.2f;
                Destroy(transform.root.gameObject, 0.5f);
                alive = false;
            }
            else
            {
                GetComponent<Shake>().shakeDuration = 0.2f;
                Destroy(gameObject, 0.5f);
                alive = false;
            }
        }
    }

}
