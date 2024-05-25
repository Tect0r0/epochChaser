using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input del jugador para Unity.InputSystem
    private Rigidbody2D rb; // Rigidbody del personaje
    private GameObject prehistoric, medieval, wildwest, modern, future; // GameObject de la era prehistorica
    public float movementSpeed, jumpForce, dashSpeed, dashDuration; // Velocidad de movimiento, fuerza de salto, velocidad del dash, duracion del dash
    public float yVelocity, xVelocity;
    private Vector2 newPosition, direction2D; // Nueva posicion, direccion 2D, 
    public bool isJumping = false; // Flag de salto
    public bool isGrounded = false; // Flag de suelo
    public bool canDoubleJump, canDash = false; // Flag de doble salto, flag de dash
    public bool isDashing = false; // Flag de dash
    public bool dJump, wJump, gHook, dash = false; // Flags de habilidades



    void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 15.0f;
        movementSpeed = 5.0f;
        dashSpeed = 40.0f;
        dashDuration = 0.2f;

        if (dash) { canDash = true; } // Check if the player can dash

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Futuristic") // Check for scene (only for last scene)
        {
            Debug.Log("Futuristic scene");
            // Find the GameObjects
            prehistoric = GameObject.Find("Prehistoric");
            medieval = GameObject.Find("Medieval");
            wildwest = GameObject.Find("WildWest");
            modern = GameObject.Find("Modern");
            future = GameObject.Find("Future");
            // Deactivate all GameObjects
            prehistoric.SetActive(false);
            medieval.SetActive(false);
            wildwest.SetActive(false);
            modern.SetActive(false);
            future.SetActive(true);

            // Assign the ChangeEpoch method to the Time input
            input.actions["Time"].started += context =>
            {
                string controlName = context.control.name; // Get the name of the control
                char lastChar = controlName[controlName.Length - 1]; // Get the last character of the name [eg: 'Keyboard/1' -> '1']

                float value;

                if (float.TryParse(lastChar.ToString(), out value)) // Try to parse the last character as a float
                {
                    // Pass the value to the ChangeEpoch method
                    ChangeEpoch(value);
                }
            };
        }

        input.actions["Jump"].started += _ => HandleJump();

        input.actions["Pause"].started += _ => Pause();

        input.actions["Dash"].started += _ => HandleDash();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction2D = input.actions["Move"].ReadValue<Vector2>();
        direction2D.Normalize();

        if (isDashing) { return; } // Check if the player is dashing (to avoid movement)

        if (!isJumping) { rb.gravityScale = 1.0f; }
        // Read player input

        yVelocity = rb.velocity.y;
        xVelocity = rb.velocity.x;

        // Move the character
        rb.velocity = new Vector2(direction2D.x * movementSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        if (isDashing) { return; } // Check if the player is dashing (to avoid jumping

        if (isGrounded || (dJump && canDoubleJump))
        { // Check if the player is grounded or has double jump
            Jump();
        }
    }


    void Jump()
    {
        if (isGrounded || canDoubleJump)
        {
            if (!isGrounded) { canDoubleJump = false; } // Disable double jump if it was used
            if (isGrounded)
            {
                if (dJump)
                {
                    canDoubleJump = true;
                }
            } // Enable double jump if it was grounded and has double jump

            isJumping = true;
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
            if (dJump) { canDoubleJump = true; }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isJumping = true;
        }
    }

    private void HandleDash() // Check if the player can dash
    {
        if (dash && canDash) { StartCoroutine(Dash()); }
    }

    IEnumerator Dash() // Coroutine for the dash
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale; // Store the original gravity
        rb.gravityScale = 0; // Set the gravity to 0

        rb.velocity = new Vector2(direction2D.x, direction2D.y > 0 ? direction2D.y : 0) * dashSpeed;
        yield return new WaitForSeconds(dashDuration); // Wait for the dash duration

        isDashing = false;
        rb.gravityScale = originalGravity; // Reset the gravity
        rb.velocity = new Vector2(0, 0); // Reset the velocity
        yield return new WaitForSeconds(1.0f); // Wait for 1 second (cooldown)
        canDash = true;
    }

    void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    void ChangeEpoch(float epoch)
    {
        // Deactivate all GameObjects
        prehistoric.SetActive(false);
        medieval.SetActive(false);
        wildwest.SetActive(false);
        modern.SetActive(false);
        future.SetActive(false);

        // Activate the correct GameObject
        switch (epoch)
        {
            case 1:
                Debug.Log("Dinos >:v");
                prehistoric.SetActive(true);
                break;
            case 2:
                Debug.Log("Knights >:)");
                medieval.SetActive(true);
                break;
            case 3:
                Debug.Log("Cowboys >:D");
                wildwest.SetActive(true);
                break;
            case 4:
                Debug.Log("Cars >:O");
                modern.SetActive(true);
                break;
            case 5:
                Debug.Log("Robots >:X");
                future.SetActive(true);
                break;
        }
    }

}
