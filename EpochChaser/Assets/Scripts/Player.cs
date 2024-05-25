using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input del jugador para Unity.InputSystem
    private Rigidbody2D rb; // Rigidbody del personaje
    public float movementSpeed; // Velocidad del personaje
    public float jumpForce; // Fuerza de salto
    public bool isJumping = false; // Flag de salto
    public bool isGrounded = false; // Flag de suelo


    void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 10.0f;
        movementSpeed = 5.0f;

        input.actions["Jump"].started += _ => Jump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isJumping) { rb.gravityScale = 1.0f; }
        // Read player input
        Vector2 direction2D = input.actions["Move"].ReadValue<Vector2>();
        direction2D.Normalize();

        // Move the character
        rb.velocity = new Vector2(direction2D.x * movementSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            rb.gravityScale = 3.0f;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

}
