using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInput input; // Input del jugador para Unity.InputSystem
    private Rigidbody2D rb; // Rigidbody del personaje
    private GameObject prehistoric; // GameObject de la era prehistorica
    private GameObject medieval; // GameObject de la era medieval
    private GameObject wildwest; // GameObject de la era del oeste
    private GameObject modern; // GameObject de la era moderna
    private GameObject future; // GameObject del futuro
    public float movementSpeed; // Velocidad del personaje
    public float jumpForce; // Fuerza de salto
    public bool isJumping = false; // Flag de salto
    public bool isGrounded = false; // Flag de suelo


    void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 15.0f;
        movementSpeed = 5.0f;

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Futuristic") // Check for scene
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

        input.actions["Jump"].started += _ => Jump();

        input.actions["Pause"].started += _ => Pause();
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
