using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomText : MonoBehaviour
{
    public string[] possibleSpeech;
    public TMPro.TextMeshPro textBox;
    // Start is called before the first frame update
    void Start()
    {
        string selectedSpeech = possibleSpeech[Random.Range(0, possibleSpeech.Length)];
        textBox.text = selectedSpeech;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
