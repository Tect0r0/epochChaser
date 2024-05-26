using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input del jugador para Unity.InputSystem
    private CameraMovement cameraMovement;
    public Camera camera;
    private Rigidbody2D rb, rb2; // Rigidbody del personaje
    private GameObject prehistoric, medieval, wildwest, modern, future; // GameObject de la era prehistorica
    public GameObject playerSpawner; // GameObject del spawner
    public float movementSpeed, jumpForce, dashSpeed, dashDuration; // Velocidad de movimiento, fuerza de salto, velocidad del dash, duracion del dash
    private Vector2 newPosition, direction2D, lastCheckpoint, HookForce, mousePosition, direction; // Nueva posicion, direccion 2D, ultimo CP
    public bool isJumping, isGrounded, isDashing, isRespawning, isCinematic = false; // Flags para movimiento
    public bool canDoubleJump, canDash, canSwitch, canHook = false; // Flags de habilidades
    public bool dJump, wJump, gHook, dash = false; // Flags de habilidades
    public bool bossDefeated = false;
    public float distance;
    public GameObject HookPrefab;
    public float ForceDistnace;
    private Hook HookScript;
    private SpriteRenderer sprite;
    public GameObject HUDText;
    public GameObject pauseUI;
    public GameObject pauseButton;
    public GameObject end;
    private TextMeshProUGUI hudText;
    public Animator animator;

    void Awake()
    {
        hudText = HUDText.GetComponent<TextMeshProUGUI>();
        input = GetComponent<PlayerInput>();
        sprite = GetComponent<SpriteRenderer>();
        HookScript = HookPrefab.GetComponent<Hook>();
        camera = Camera.main;
        cameraMovement = camera.GetComponent<CameraMovement>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 15.0f;
        movementSpeed = 5.0f;
        dashSpeed = 40.0f;
        dashDuration = 0.2f;
        lastCheckpoint = new Vector2(0f, 0f);
        isDashing = false;
        lastCheckpoint = playerSpawner.transform.position;
        rb2 = HookScript.GetComponent<Rigidbody2D>();
        pauseUI.SetActive(false);
        animator = GetComponent<Animator>();

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "ZZZSampleScene")
        {
            dJump = true;
            dash = true;
            gHook = true;
        }

        if (sceneName == "Futuristic") // Check for scene (only for last scene)
        {
            canSwitch = true;
            dJump = true;
            dash = true;
            gHook = true;
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
        if (gHook) { canHook = true; } // Check if the player can hook

        input.actions["Jump"].started += _ => HandleJump();

        input.actions["Pause"].started += _ => Pause();

        input.actions["Dash"].started += _ => HandleDash();

        input.actions["Shoot"].started += _ => Shoot();

        input.actions["Hook"].started += _ => HandleHook();

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

        if (rb.velocity != Vector2.zero) { animator.SetBool("isWalking", true); }
        else { animator.SetBool("isWalking", false); }

        // Flip the sprite if moving left or right
        if (direction2D.x > 0) { sprite.flipX = false; }
        else if (direction2D.x < 0) { sprite.flipX = true; }

        if (bossDefeated) { BossDefeated(); }
    }

    void HandleJump()
    {
        if (isDashing || isRespawning || isCinematic) { return; } // Check if the player is dashing (to avoid jumping

        if (isGrounded || (dJump && canDoubleJump)) { Jump(); }
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
            animator.SetBool("isJumping", true);
            rb.gravityScale = 3.0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    Vector2 GetDirection(Vector2 fromPosition, Vector2 toPosition)
    {
        direction = toPosition - fromPosition;
        return direction.normalized;
    }

    public void HookUse(Vector2 hookablePos)
    {
        canDash = true;
        canDoubleJump = true;
        StartCoroutine(HookMovement(hookablePos));
    }

    IEnumerator HookMovement(Vector2 hookablePos)
    {
        transform.position = new Vector2(hookablePos.x, hookablePos.y);
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1f);
    }

    void HandleHook()
    {
        if (gHook && !isRespawning && !isCinematic && canHook) { StartCoroutine(HookLaunch()); }
    }

    IEnumerator HookLaunch()
    {
        canHook = false;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = GetDirection(rb.transform.position, mousePosition);
        HookForce = direction * 5;
        GameObject newObject = Instantiate(HookPrefab, rb.position, Quaternion.identity);
        newObject.transform.position = new Vector2(newObject.transform.position.x, newObject.transform.position.y);

        Rigidbody2D rbHookPrefab = newObject.GetComponent<Rigidbody2D>();
        rbHookPrefab.AddForce(HookForce * 5, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        canHook = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")) // Checar si toca el suelo
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("isJumping", false);
            if (dJump) { canDoubleJump = true; }
        }
        // Checkpoints
        if (other.gameObject.CompareTag("DeathPlane")) { StartCoroutine(Respawn()); }
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
            HUDText.SetActive(false);
            ChangeEpoch(5);
            canSwitch = false;
        }
        if (other.gameObject.name == "CinematicTrigger")
        {
            isCinematic = true;
            StartCoroutine(Cinematic1());
        }

        if (other.gameObject.name == "BossDefeatTrigger") { bossDefeated = true; }

        if (other.tag == "Dash") { dash = true; }
        if (other.tag == "DoubleJump") { dJump = true; }
        if (other.tag == "WallJump") { wJump = true; }
        if (other.tag == "GrapplingHook") { gHook = true; }
    }

    IEnumerator Cinematic1()
    {
        HUDText.SetActive(false);
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
        cameraMovement.transform.position = new Vector3(126, camera.transform.position.y, camera.transform.position.z); // Set the camera position

        canSwitch = true;
        isCinematic = false;
        HUDText.SetActive(true);
    }

    public void BossDefeated()
    {
        ChangeEpoch(5);
        transform.position = new Vector2(transform.position.x, -7);
        rb.velocity = new Vector2(0, 0);
        bossDefeated = true;
        isCinematic = true;
        canSwitch = false;
        StartCoroutine(Cinematic2());
    }

    IEnumerator Cinematic2()
    {
        HUDText.SetActive(false);
        float cinematicSpeed = 5.0f; // Set the speed for the cinematic
        float startTime = Time.time; // Record the start time
        float duration = 1.7f; // Set the duration for the cinematic

        Rigidbody2D exitDoor = GameObject.Find("ExitDoor").GetComponent<Rigidbody2D>();

        while (Time.time < startTime + duration)
        {
            exitDoor.velocity = new Vector2(0, cinematicSpeed); // Move the door down
            cameraMovement.size = cameraMovement.size -= 0.02f; // Zoom in the camera
            yield return null; // Wait for the next frame
        }
        cameraMovement.minY = -6;
        cameraMovement.maxX = 186;
        cameraMovement.following = true; // Stop the camera from following the player
        exitDoor.velocity = new Vector2(0, 0); // Stop the door

        cinematicSpeed = 5.0f; // Set the speed for the cinematic
        duration = 8.3f; // Set the duration for the cinematic
        startTime = Time.time; // Record the start time

        while (camera.transform.position.x < 185.0f)
        {
            rb.velocity = new Vector2(cinematicSpeed, rb.velocity.y); // Move the player to the right
            yield return null; // Wait for the next frame
        }

        cameraMovement.following = false; // Stop the camera from following the player
        while (rb.transform.position.x < 187.4)
        {
            rb.velocity = new Vector2(cinematicSpeed, rb.velocity.y); // Move the player to the right
            yield return null; // Wait for the next frame
        }
        rb.velocity = new Vector2(0, rb.velocity.y); // Stop the player
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeOut(sprite, 1.5f));

        // End
        end.SetActive(true);
        string[] FinalNames = { "EndScreen", "Thanks", "G&M", "G&MText", "Audio", "AudioText", "Visuals", "VisualsText", "Quit", "QuitText" };

        foreach (string name in FinalNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
                Image image = obj.GetComponent<Image>();

                if (text != null)
                {
                    // Handle TextMeshProUGUI
                    yield return StartCoroutine(FadeIn(text, 2.5f));
                }
                else if (image != null)
                {
                    // Handle Image
                    yield return StartCoroutine(FadeIn(image, 2.5f));
                }
            }
        }
    }

    IEnumerator FadeIn(Graphic graphic, float duration)
    {
        float rate = 1.0f / (duration * 60.0f);  // Assuming 60 frames per second
        while (graphic.color.a < 1)
        {
            Color color = graphic.color;
            color.a += rate;
            graphic.color = color;
            yield return null;  // Wait for the next frame
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1);
    }

    IEnumerator FadeOut(SpriteRenderer sprite, float duration)
    {
        float rate = 1.0f / (duration * 60.0f);  // Assuming 60 frames per second
        while (sprite.color.a > 0)
        {
            Color color = sprite.color;
            color.a -= rate;
            sprite.color = color;
            yield return null;  // Wait for the next frame
        }
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);  // Ensure opacity is 0
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
        animator.SetBool("isDashing", true);

        float originalGravity = rb.gravityScale; // Store the original gravity
        rb.gravityScale = 0; // Set the gravity to 0

        rb.velocity = new Vector2(direction2D.x, direction2D.y > 0 ? direction2D.y : 0) * dashSpeed;
        yield return new WaitForSeconds(dashDuration); // Wait for the dash duration

        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.gravityScale = originalGravity; // Reset the gravity
        rb.velocity = new Vector2(0, 0); // Reset the velocity
        yield return new WaitForSeconds(0.7f); // Wait for 1 second (cooldown)
        canDash = true;
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pauseUI.SetActive(!pauseUI.activeSelf);
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
                hudText.text = "Prehistoric";
                hudText.color = Color.red;
                pauseButton.GetComponent<Image>().color = Color.red;
                break;
            case 2:
                Debug.Log("Knights >:)");
                medieval.SetActive(true);
                hudText.text = "Medieval";
                hudText.color = Color.gray;
                pauseButton.GetComponent<Image>().color = Color.gray;
                break;
            case 3:
                Debug.Log("Cowboys >:D");
                wildwest.SetActive(true);
                hudText.text = "Wild West";
                hudText.color = Color.yellow;
                pauseButton.GetComponent<Image>().color = Color.yellow;
                break;
            case 4:
                Debug.Log("Cars >:O");
                modern.SetActive(true);
                hudText.text = "Modern";
                hudText.color = Color.blue;
                pauseButton.GetComponent<Image>().color = Color.blue;
                break;
            case 5:
                Debug.Log("Robots >:X");
                future.SetActive(true);
                hudText.text = "Future";
                hudText.color = Color.green;
                pauseButton.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    void Shoot() { Debug.Log("Pew pew"); }

    public void QuitGame() { Application.Quit(); }
}
