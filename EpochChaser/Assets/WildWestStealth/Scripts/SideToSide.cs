using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSide : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private int patrolState = 1;
    public int patrolDistance = 10;
    public float speed = 10f;
    private bool lookingRight = true;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
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
}
