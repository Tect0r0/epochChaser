using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int health;
    public int maxHealth = 20;
    public GameObject damageTextPrefab;
    Animator animator;
    enemyAI2D AI;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        AI = gameObject.GetComponent<enemyAI2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Die()
    {
        animator.Play("enemy_death");
        GameObject.Find(gameObject.name).GetComponent<enemyAI2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("enemy_death"));
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        GameObject damageText = Instantiate(damageTextPrefab, transform);
        damageText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(damage.ToString());
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

}
