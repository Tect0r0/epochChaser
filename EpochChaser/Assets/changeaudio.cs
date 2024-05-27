using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeaudio : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[0];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyManager.isActive){
            if (audioSource.clip == clips[0]){
                audioSource.clip = clips[1];
                audioSource.Play();
            }
        }
    }
}
