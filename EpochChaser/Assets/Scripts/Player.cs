using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input for the Unity Input System
    private Rigidbody2D rb; // Rigidbody of the character
    public float movementSpeed; // Character's movement speed
    public float jumpForce; // Jump force
    public bool isJumping = false; // Jump flag
    public bool isGrounded = false; // Grounded flag
    public bool dJump, wJump, gHook, dash = false;

    private bool canDoubleJump = false;
    public float dashSpeed;
    public float dashDuration = 0.2f; // Duration of the dash
    private Vector2 newPosition;
    public KeyCode ButtonW = KeyCode.W;
    public KeyCode ButtonA = KeyCode.A;
    public KeyCode ButtonS = KeyCode.S;
    public KeyCode ButtonD = KeyCode.D;
    public KeyCode ButtonSpace = KeyCode.Space;
    public float yVelocity, xVelocity;
    



    void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 10.0f;
        movementSpeed = 5.0f;
    }

    void Update()
    {
        if (!isJumping) { rb.gravityScale = 1.0f; }

        // Read player input
        Vector2 direction2D = input.actions["Move"].ReadValue<Vector2>();
        direction2D.Normalize();

        yVelocity = rb.velocity.y;
        xVelocity = rb.velocity.x;

        // Move the character
        rb.velocity = new Vector2(direction2D.x * movementSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.W))
        {
            HandleJump();
        }

        if (Input.GetKeyDown(ButtonSpace))
        {
            StartCoroutine(Dash());
        }
    }

    void HandleJump()
    {
        if (isGrounded || canDoubleJump)
        {
            Jump();
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f); // Wait
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isJumping = true;
            isGrounded = false;
            rb.gravityScale = 3.0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canDoubleJump = !canDoubleJump; // Reset the double jump ability
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    IEnumerator Dash()
    {
        Vector2 dashDirection = Vector2.zero;
        
        if (Input.GetKey(ButtonW))
        {
            dashDirection = Vector2.up;
        }
        else if (Input.GetKey(ButtonS))
        {
            dashDirection = Vector2.down;
        }
        else if (Input.GetKey(ButtonA))
        {
            dashDirection = Vector2.left;
        }
        else if (Input.GetKey(ButtonD))
        {
            dashDirection = Vector2.right;
        }

        if (dashDirection != Vector2.zero)
        {
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = startPosition + dashDirection * dashSpeed;

            float elapsedTime = 0f;

            while (elapsedTime < dashDuration)
            {
                transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yVelocity = 0f;
            

            transform.position = targetPosition; 
            rb.velocity = new Vector2(xVelocity, yVelocity);// Ensure the final position is set
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Dash") { dash = true; }
        if (other.tag == "DoubleJump") { dJump = true; }
        if (other.tag == "WallJump") { wJump = true; }
        if (other.tag == "GrapplingHook") { gHook = true; }
    }
}
