using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    // Player Stats
    public float health;
    public float speed;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
    public float minStamina;

    // UI
    public Text healthText;
    public Image staminaBar;
    public Image speedIndicator;
    public Canvas canvasMain;
    public Canvas canvasPause;
    public Canvas canvasMati;

    // Assignables
    public Transform playerCam;
    public Transform orientation;
    public GameObject forceFieldPrefab;

    // Other
    private Rigidbody rb;
    private bool isBoosting = false;
    private Vector3 normalVector = Vector3.up;
    private float damage;

    // Movement
    public float walkSpeed = 7f;
    public float runSpeed = 15f;
    public float boostSpeed = 20f;
    public float maxSpeed = 20f;
    private bool grounded = false;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;

    // Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    // Camera Shake
    private float walkShakeAmount = 0.02f;
    private float runShakeAmount = 0.08f;
    private float shakeFrequency = 1.5f;
    private Vector3 initialCamPosition;

    // Input
    private float x, y;
    bool jumping, crouching, sprinting;

    // Audio
    public AudioClip stepSound;
    public AudioClip Drop;
    private AudioSource audioSource;
    private bool isPlayingStepSound = false;

    // Flashlight
    public Light Senter;
    public bool Senternya = false;

    public Transform groundCheck;
    public float groundDistance = 0.2f;

    // Declare camera rotation variables
    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCam.gameObject.SetActive(true);
        canvasPause.gameObject.SetActive(false);
        initialCamPosition = playerCam.localPosition;
        canvasMati.gameObject.SetActive(false);
        Senter.gameObject.SetActive(Senternya);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Exp"))
        {
            health -= damage;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            readyToJump = true;
            grounded = true;
            audioSource.PlayOneShot(Drop);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            readyToJump = false;
            grounded = false;
        }
    }

    private void Update()
    {
        MyInput();
        Look();
        SmoothCameraShake();
        PlayStepSound();
        UpdateUI();
        Die();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.C);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        if (sprinting)
        {
            stamina -= Time.deltaTime * 10f;
        }
        else if (sprinting && stamina > minStamina)
        {
            StartBoost();
        }
        else
        {
            StopBoost();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            TogglePause();
        }

        senter();

        if (Input.GetKeyDown(KeyCode.C))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.C))
            StopCrouch();
    }

    private void StartCrouch()
    {
        if (sprinting && grounded)
        {
            Vector3 slideDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
            rb.AddForce(slideDirection * runSpeed * 0.001f, ForceMode.VelocityChange);
        }

        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void StartBoost()
    {
        isBoosting = true;
        rb.drag = 0.00000000002f;
        if (stamina > minStamina)
        {
            stamina -= Time.deltaTime * 10f;
        }
    }

    private void StopBoost()
    {
        isBoosting = false;
        rb.drag = 3f;
        if (stamina < maxStamina)
        {
            stamina += Time.deltaTime * 5f;
        }
    }

    private void Movement()
    {
        float currentSpeed = isBoosting ? boostSpeed : (sprinting ? runSpeed : walkSpeed);
        if (readyToJump && jumping) Jump();

        float multiplier = grounded ? 1f : 0.5f;
        rb.AddForce(orientation.transform.forward * y * currentSpeed * Time.deltaTime * multiplier);
        rb.AddForce(orientation.transform.right * x * currentSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;
            rb.AddForce(Vector3.up * jumpForce * 2f);
        }
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * 2.0f;
        float mouseY = Input.GetAxis("Mouse Y") * 2.0f;

        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);
        yRotation += mouseX;

        playerCam.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void SmoothCameraShake()
    {
        if (grounded && (Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f))
        {
            float shakeAmount = sprinting ? runShakeAmount : walkShakeAmount;
            Vector3 targetPosition = initialCamPosition + new Vector3(
                Mathf.Sin(Time.time * shakeFrequency) * shakeAmount,
                Mathf.Sin(Time.time * shakeFrequency * 0.5f) * shakeAmount,
                0);

            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, targetPosition, Time.deltaTime * 5f);
        }
        else
        {
            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, initialCamPosition, Time.deltaTime * 5f);
        }
    }

    private void PlayStepSound()
    {
        if ((Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = stepSound;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void UpdateUI()
    {
        healthText.text = health.ToString();
        staminaBar.fillAmount = stamina / maxStamina;
    }

    public void TogglePause()
    {
        bool isPaused = !canvasPause.gameObject.activeSelf;
        canvasPause.gameObject.SetActive(isPaused);
        canvasMain.gameObject.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void Die()
    {
        if (health <= 0)
        {
            rb.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            this.enabled = false;

            canvasMati.gameObject.SetActive(true);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None; // Unlock cursor
            Cursor.visible = true;                 // Show cursor
        }
    }

    private void senter()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Senternya = !Senternya;
            Senter.gameObject.SetActive(Senternya);
        }
    }
}
