using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage = 10;
    EnemyStats stats;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            stats = collision.GetComponent<EnemyStats>();
            stats.TakeDamage(damage);
        }
    }
}
