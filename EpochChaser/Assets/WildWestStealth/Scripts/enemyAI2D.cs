using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI2D : MonoBehaviour
{

    // Definir las variables
    public float lookRadius = 10f;
    public Transform target;
    public float speed = 10f;
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    private float fireTime = 0f;
    private Vector2 direction;
    public float projectileSpeed = 20f;
    public float detectionDelay = 1f;
    private float detectionTime = 0f;
    public Slider detectionBar;
    private int detectionLevel = 0;
    private bool enemyDetected = false;
    public GameObject EnemyText;
    private bool lookingRight = true;
    public int patrolDistance;
    public float personalSpaceRadius = 1.5f;
    public float repulsionForce = 2f;
    private Vector3 startPosition;
    [SerializeField] private int patrolState = 1;
    private bool isLookingAtPlayer = false;
    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        //Referencia al objetivo(jugador)
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        detectionBar.value = detectionLevel;
        //Calcular distancia
        float distance = Vector2.Distance(target.position, transform.position);

        if (enemyDetected)
        {
            if (!EnemyManager.isDiscovered)
            {
                EnemyManager.AlertOthers();
            }
            //vector entre el objetivo y el enemigo
            direction = (target.position - transform.position).normalized;

            //Formula para que dispare de acuerdo al fire rate
            if (Time.time > fireTime)
            {
                int randomChange = Random.Range(0, 101);
                if (randomChange > 90)
                {
                    EnemyManager.throwDynamite = true;
                }
                //Cambiar atributos dependiendo de la dificultad
                fireTime = Time.time + fireRate;
                Shoot();
            }
        }

        else
        {
            Patrol();
            CheckIfIsLookingAtTarget();
            CheckForEnemies(distance);
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = direction * projectileSpeed;
    }

    void CheckIfIsLookingAtTarget()
    {
        if ((EnemyManager.isVisible && lookingRight && (target.transform.position.x > transform.position.x)) || EnemyManager.isVisible && (!lookingRight && (target.transform.position.x < transform.position.x)))
        {
            isLookingAtPlayer = true;
        }
        else
        {
            isLookingAtPlayer = false;
        }
    }

    void Patrol()
    {
        switch (patrolState)
        {
            case 1:
                Vector2 endRightPosition = startPosition;
                endRightPosition.x = startPosition.x + patrolDistance;
                transform.position = Vector2.MoveTowards(transform.position, endRightPosition, speed * Time.deltaTime);
                if (transform.position.x == endRightPosition.x)
                {
                    patrolState = 2;
                    Flip();
                }
                break;
            case 2:
                Vector2 endLeftPosition = startPosition;
                endLeftPosition.x = startPosition.x - patrolDistance;
                transform.position = Vector2.MoveTowards(transform.position, endLeftPosition, speed * Time.deltaTime);
                if (transform.position.x == endLeftPosition.x)
                {
                    patrolState = 1;
                    Flip();
                }
                break;

            case 3:
                break;
        }
    }
    void CheckForEnemies(float distance)
    {
        if (EnemyManager.isDiscovered)
        {
            detectionLevel = 20;
        }
        
        if (detectionLevel <= 0)
        {
            detectionLevel = 0;
            detectionBar.gameObject.SetActive(false);
        }
        else
        {
            detectionBar.gameObject.SetActive(true);
        }

        if (distance <= lookRadius)
        {
            if (Time.time > detectionTime && !(detectionLevel >= 20) && isLookingAtPlayer)
            {
                detectionTime = Time.time + detectionDelay;
                detectionLevel += 1;
            }

            if (detectionLevel >= 20)
            {
                detectionBar.gameObject.SetActive(false);
                enemyDetected = true;
                EnemyText.SetActive(true);
            }
        }

        if (distance >= lookRadius || !isLookingAtPlayer)
        {
            if (Time.time > detectionTime)
            {
                detectionTime = Time.time + detectionDelay;
                detectionLevel -= 1;
            }
        }
    }

    void Flip()
    {
        lookingRight = !lookingRight;
        if (lookingRight)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        //color y ubicacion con el radio del circulo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, personalSpaceRadius);
    }


}
