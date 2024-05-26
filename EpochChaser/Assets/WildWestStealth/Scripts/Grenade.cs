using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Grenade : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float explosionForce = 100f;
    AudioSource sfx;
    Rigidbody2D rb;
    CircleCollider2D explosionCollider;
    SpriteRenderer spriteRenderer;
    Grenade explosionCode;
    public int damage = 10;
    int finalDamage;
    Animator grenadeAnim;
    bool isExploded = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(10f));
        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        explosionCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionCode = GetComponent<Grenade>();
        grenadeAnim = GetComponent<Animator>();
        
    }

    IEnumerator HideAfterAnimation()
    {
        yield return new WaitForSeconds(grenadeAnim.GetCurrentAnimatorStateInfo(0).length);
        spriteRenderer.enabled = false;
    }

    void Explosion()
    {
        isExploded = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        //Animar la explosión y crear el area de impacto
        transform.localScale = new Vector3(explosionRadius, explosionRadius, 0);
        grenadeAnim.Play("explosion");
        StartCoroutine(HideAfterAnimation());
        if (!sfx.isPlaying)
        {
            sfx.Play();
        }
        Vector2 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            //Calcular fuerza y distancia para cada objeto en la explosión
            Vector2 explosionDirection = collider.transform.position - transform.position;
            float distance = explosionDirection.magnitude;
            if (collider.CompareTag("Enemy"))
            {
                //Hacer que el enemigo tome daño
                EnemyStats stats = collider.GetComponent<EnemyStats>();
                stats.TakeDamage(damage);
            }
            if (collider.CompareTag("Enemy Bullet"))
            {
                Destroy(collider.gameObject);
            }
            
            float finalForce = explosionForce / distance;
            if (distance == 0)
            {
                finalDamage = damage;
            }
            if (distance >= 1)
            {
                finalDamage = damage / ((int)distance);
            }

        }
        
        explosionCollider.enabled = false;
        explosionCode.enabled = false;

    }

    IEnumerator DestroyAfterTime(float time)
    {
        if (!isExploded)
        {
            yield return new WaitForSeconds(time);
            Explosion();
        }
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explosion();
    }

    private void OnDrawGizmosSelected()
    {
        //color y ubicacion con el radio del circulo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);


    }

}
