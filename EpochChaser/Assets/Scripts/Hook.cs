using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private Rigidbody2D rb;
    private Rigidbody2D rb2;
    private Player PlayerScript;
    public float distance;
    private float maxHookDistance;

    // Start is called before the first frame update
    void Awake()
    {
        maxHookDistance = 20f;
        PlayerScript = GameObject.Find("Player").GetComponent<Player>();
        rb = PlayerScript.GetComponent<Rigidbody2D>();
        rb2 = GetComponent<Rigidbody2D>();
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
        if (distance > maxHookDistance)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hookable")
        {
            Vector2 position2D = new Vector2(other.transform.position.x, other.transform.position.y);
            PlayerScript.HookUse(position2D);
            Destroy(this.gameObject);
        }
        if (other.tag == "DeathPlane" || other.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}