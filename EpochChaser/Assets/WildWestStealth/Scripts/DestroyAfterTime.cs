using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeToDestroy = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(timeToDestroy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
