using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemies;
    public int maxEnemies;
    public float spawnRate;
    int totalEnemies = 0;
    float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnTime && totalEnemies < maxEnemies)
        {
            spawnTime = Time.time + spawnRate;
            Instantiate(Enemies, transform.position, Quaternion.identity);
            totalEnemies += 1;

        }
    }
}
