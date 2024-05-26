using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetScope : MonoBehaviour
{
    Transform target;
    Player playerScript;
    public float speed = 5f;
    public float timeToShoot = 5f;
    public AudioClip[] clips;
    public AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        audioSource.clip = clips[0];
        StartCoroutine(DeadEye());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void Shoot()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
        if (EnemyManager.isOnSight)
        {
            Debug.Log("bang");
            //playerScript.Die();
        }
        spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    IEnumerator DeadEye()
    {
        yield return new WaitForSeconds(timeToShoot);
        Shoot();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
