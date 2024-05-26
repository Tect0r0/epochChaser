using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input del jugador para Unity.InputSystem
    public CameraMovement cameraMovement;
    private Rigidbody2D rb, rb2; // Rigidbody del personaje
    private GameObject prehistoric, medieval, wildwest, modern, future; // GameObject de la era prehistorica
    public GameObject playerSpawner; // GameObject del spawner
    public float movementSpeed, jumpForce, dashSpeed, dashDuration; // Velocidad de movimiento, fuerza de salto, velocidad del dash, duracion del dash
    private Vector2 newPosition, direction2D, lastCheckpoint, HookForce, mousePosition, direction; // Nueva posicion, direccion 2D, ultimo CP
    public bool isJumping, isGrounded, isDashing, isRespawning, isCinematic = false; // Flags para movimiento
    public bool canDoubleJump, canDash, canSwitch = false; // Flags de habilidades
    public bool dJump, wJump, gHook, dash = false; // Flags de habilidades
    public float distance;
    public GameObject prefab;
    public float ForceDistnace;
    public GameObject HookScript;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 15.0f;
        movementSpeed = 5.0f;
        dashSpeed = 40.0f;
        dashDuration = 0.2f;
        lastCheckpoint = new Vector2(0f, 0f);
        isDashing = false;
        lastCheckpoint = playerSpawner.transform.position;
        rb2 = HookScript.GetComponent<Rigidbody2D>();

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Futuristic") // Check for scene (only for last scene)
        {
            canSwitch = true;
            dJump = true;
            dash = true;
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
                char lastChar = controlName[controlName.Length - 1]; // Get the last character of the name [eg: 'Keyboard/1' -> '1'] -> "1"

                if (float.TryParse(lastChar.ToString(), out float value)) // Try to parse the last character as a float
                {
                    // Pass the value to the ChangeEpoch method
                    ChangeEpoch(value);
                }
            };
        }

        if (dash) { canDash = true; } // Check if the player can dash
        if (dJump) { canDoubleJump = true; } // Check if the player can double jump

        input.actions["Jump"].started += _ => HandleJump();

        input.actions["Pause"].started += _ => Pause();

        input.actions["Dash"].started += _ => HandleDash();

        input.actions["Shoot"].started += _ => Shoot();

        input.actions["Hook"].started += _ => HookLaunch();

        transform.position = lastCheckpoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDashing || isRespawning || isCinematic) { return; } // Check if the player is dashing (to avoid movement)
        // Read player input
        direction2D = input.actions["Move"].ReadValue<Vector2>(); // Vector2D de movimiento
        // Move the character
        rb.velocity = new Vector2(direction2D.x * movementSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        if (isDashing || isRespawning || isCinematic) { return; } // Check if the player is dashing (to avoid jumping

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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
    }

    Vector2 GetDirection(Vector2 fromPosition, Vector2 toPosition)
    {
        direction = toPosition - fromPosition;
        return direction.normalized;
    }

    public void HookUse()
    {
        rb.AddForce(HookForce, ForceMode2D.Impulse);
    }

    void HookLaunch()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = GetDirection(rb.transform.position, mousePosition);
        HookForce.x = direction.x * distance;
        HookForce.y = direction.y * distance;
        GameObject newObject = Instantiate(prefab, rb.position, Quaternion.LookRotation(direction));
        Rigidbody2D rbPrefab = newObject.GetComponent<Rigidbody2D>();
        rbPrefab.AddForce(HookForce * 5, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")) // Checar si toca el suelo
        {
            isGrounded = true;
            isJumping = false;
            if (dJump) { canDoubleJump = true; }
        }

        if (other.gameObject.CompareTag("DeathPlane")) // Checkpoints
        {
            Debug.Log("You died");
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")) // Checar si deja de tocar el suelo
        {
            isGrounded = false;
            isJumping = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Checkpoint")) // Checkpoints
        {
            lastCheckpoint = other.transform.position;
            Debug.Log("Checkpoint reached");
        }
        if (other.gameObject.name == "SwitchTriggerOff")
        {
            Debug.Log("Switchn't");
            ChangeEpoch(5);
            canSwitch = false;
        }
        if (other.gameObject.name == "CinematicTrigger")
        {
            isCinematic = true;
            StartCoroutine(Cinematic1());
        }

        if (other.tag == "Dash") { dash = true; }
        if (other.tag == "DoubleJump") { dJump = true; }
        if (other.tag == "WallJump") { wJump = true; }
        if (other.tag == "GrapplingHook") { gHook = true; }
    }

    IEnumerator Cinematic1()
    {
        float cinematicSpeed = 5.0f; // Set the speed for the cinematic
        float duration = 8.3f; // Set the duration for the cinematic
        float startTime = Time.time; // Record the start time

        while (Time.time < startTime + duration)
        {
            rb.velocity = new Vector2(cinematicSpeed, rb.velocity.y); // Move the player to the right
            yield return null; // Wait for the next frame
        }
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop the player
        Rigidbody2D door = GameObject.Find("Door").GetComponent<Rigidbody2D>();

        duration = 1.7f; // Set the duration for the cinematic
        startTime = Time.time; // Record the start time

        while (Time.time < startTime + duration)
        {
            door.velocity = new Vector2(0, -cinematicSpeed); // Move the door down
            cameraMovement.size = cameraMovement.size += 0.004f; // Zoom in the camera
            yield return null; // Wait for the next frame
        }
        door.velocity = new Vector2(0, 0); // Stop the door
        cameraMovement.following = false; // Stop the camera from following the player
        cameraMovement.minX = 124; // Set the boundaries for the camera
        cameraMovement.maxX = 124;

        yield return new WaitForSeconds(3.0f);
        canSwitch = true;
        isCinematic = false;
    }

    IEnumerator Respawn() // Coroutine for the respawn
    {
        isRespawning = true;
        yield return new WaitForSeconds(1.0f); // Timer
        transform.position = lastCheckpoint;
        isRespawning = false;
    }

    private void HandleDash() // Check if the player can dash
    {
        if (dash && canDash && direction2D.x != 0 && !isRespawning && !isCinematic) { StartCoroutine(Dash()); }
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
        yield return new WaitForSeconds(0.7f); // Wait for 1 second (cooldown)
        canDash = true;
    }

    void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    void ChangeEpoch(float epoch)
    {
        if (!canSwitch) { return; }
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

    void Shoot()
    {
        Debug.Log("Pew pew");
    }
}
