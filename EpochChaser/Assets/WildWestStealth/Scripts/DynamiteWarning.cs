using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DynamiteWarning : MonoBehaviour
{
    public Transform target;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float dropCooldown = 2f;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = target.position.x;
        transform.position = new Vector3(x, transform.position.y, 1);
        if (EnemyManager.throwDynamite)
        {
            EnemyManager.throwDynamite = false;
            spriteRenderer.enabled = true;
            StartCoroutine(Warning());
        }
    }

    IEnumerator Warning()
    {
        Debug.Log("Start Warning");
        yield return new WaitForSeconds(dropCooldown);
        DropDynamite();
    }

    void DropDynamite()
    {
        Vector2 direction = new (0, -1);
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = direction * projectileSpeed;
        spriteRenderer.enabled = false;
    }
}
