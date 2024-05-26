using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioSource;
    [SerializeField] private int totalShots = 0;
    private bool isDying = false;
    private bool isStarted = false;
    private bool isStarting = false;
    public GameObject targetPrefab;
    public float throwRate = 1f;
    private float throwTime = 0f;
    public float fireRate = 5f;
    private float fireTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyManager.isActive && !isDying)
        {
            if (!isStarted && !isStarting)
            {
                StartCoroutine(Intro());
            }
            else if (totalShots < 6 && isStarted)
            {
                if (Time.time > throwTime)
                {
                    int randomChange = Random.Range(0, 101);
                    if (randomChange > 70)
                    {
                        EnemyManager.throwDynamite = true;
                    }
                    throwTime = Time.time + throwRate;
                }
                if (Time.time > fireTime)
                {
                    int randomChange = Random.Range(0, 101);
                    if (randomChange > 90)
                    {
                        EnemyManager.throwDynamite = true;
                    }
                    fireTime = Time.time + fireRate;
                    SpawnTarget();
                    totalShots += 1;
                }
            }

            if (!isDying && totalShots >= 6)
            {
                StartCoroutine(Die());
            }
        }
    }

    IEnumerator Intro()
    {
        isStarting = true;
        audioSource.Play();
        yield return new WaitForSeconds(4f);
        isStarted = true;
    }

    IEnumerator Die()
    {
        isDying = true;
        yield return new WaitForSeconds(5f);
        audioSource.clip = clips[1];
        audioSource.Play();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void SpawnTarget()
    {
        Instantiate(targetPrefab, transform.position, Quaternion.identity);
    }
}
