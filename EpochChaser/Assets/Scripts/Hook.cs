using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public GameObject hook;
    private Rigidbody2D rb;
    private Rigidbody2D rb2;
    public Player PlayerScript;
    public float distance;

    // Start is called before the first frame update
    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        rb = PlayerScript.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HookDistance();
    }

    void HookDistance()
    {
        Vector2 v1 = rb.transform.position;
        Vector2 v2 = rb2.transform.position;
        Vector2 difference = new Vector2(v1.x - v2.x, v1.y - v2.y);

        distance = Mathf.Sqrt(Mathf.Pow(difference.x, 2f) + Mathf.Pow(difference.y, 2f));


    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Hookable")
        {
            Debug.Log("Si");
            PlayerScript.HookUse(rb);
        }
    }
}