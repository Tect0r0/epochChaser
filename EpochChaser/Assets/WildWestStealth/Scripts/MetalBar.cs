using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalBar : MonoBehaviour
{
    public float explosionRadius = 3f;
    AudioSource sfx;
    SpriteRenderer barSprite;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(3f));
        sfx = GetComponent<AudioSource>();
        barSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Stun(Collider2D collider)
    {
        yield return new WaitForSeconds(2f);
        if (collider != null)
        {
            if (collider.CompareTag("Enemy"))
            {
                GameObject.Find(collider.name).GetComponent<enemyAI2D>().enabled = true;
            }
        }
        
        
    }

    void Explosion()
    {
        if (!sfx.isPlaying)
        {
            sfx.Play();
        }
        rb.velocity = Vector3.zero;
        Vector2 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy Bullet"))
            {
                Destroy(collider.gameObject);
            }
            if (collider.CompareTag("Enemy"))
            {
                GameObject.Find(collider.name).GetComponent<enemyAI2D>().enabled = false;
            }
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            StartCoroutine(Stun(collider));

        }
    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            barSprite.enabled = false;
            Explosion();
        }
    }

    private void OnDrawGizmosSelected()
    {
        //color y ubicacion con el radio del circulo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);


    }
}
