using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float distance = 1;
    [SerializeField] float parryCooldown = 1f;
    [SerializeField] float parryActiveTime = 0.5f;
    [SerializeField] float parryVelocityMultiplier = 2f;
    [SerializeField] int parryDamageMultiplier = 3;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 5f;
    public GameObject specialProjectilePrefab;
    [SerializeField] GameObject[] specialProjectilePrefabs;
    private new BoxCollider2D collider;
    private SpriteRenderer parrySprite;
    public bool parryReady = true;
    AudioSource sfx;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        parrySprite = GetComponent<SpriteRenderer>();
        specialProjectilePrefab = specialProjectilePrefabs[0];
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        LookAtMouse();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Parry();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            specialProjectilePrefab = specialProjectilePrefabs[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            specialProjectilePrefab = specialProjectilePrefabs[1];
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            sfx.Play();
            SpecialAbility();
        }

    }

    IEnumerator ParryCooldown()
    {
        yield return new WaitForSeconds(parryActiveTime);
        parryReady = false;
        parrySprite.enabled = false;
        collider.enabled = false;
        yield return new WaitForSeconds(parryCooldown);
        parryReady = true;
    }

    void LookAtMouse()
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseScreenPosition - (Vector2)player.transform.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.back, direction);
        transform.rotation = rotation;
        Vector3 newPos = new Vector3(0, distance, 0);
        newPos = rotation * newPos;
        transform.localPosition = newPos;
    }

    void Parry()
    {
        if (parryReady)
        {
            collider.enabled = true;
            parrySprite.enabled = true;
            StartCoroutine(ParryCooldown());
        }
        
    }

    void Shoot()
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseScreenPosition - (Vector2)player.transform.position;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = direction * projectileSpeed;
    }

    void SpecialAbility()
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseScreenPosition - (Vector2)player.transform.position;
        GameObject projectile = Instantiate(specialProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = direction * projectileSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy Bullet" || collision.CompareTag("Bullet"))
        {
            if (collision.CompareTag("Enemy Bullet"))
            {
                EnemyBulletScript enemy_bullet = collision.GetComponent<EnemyBulletScript>();
                enemy_bullet.isReflected = true;
                enemy_bullet.damage *= parryDamageMultiplier;
            }
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseScreenPosition - (Vector2)player.transform.position;
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(direction.x, direction.y) * rb.velocity.magnitude * parryVelocityMultiplier;
        }
    }
}
