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

    // Assignables
    public Transform playerCam;
    public Transform orientation;
    public GameObject forceFieldPrefab;

    // Camera Effects
    //public GameObject postProcessingVolume;
    //private ChromaticAberration chromaticAberration;
    //private LensDistortion lensDistortion;

    // Other
    private Rigidbody rb;
    private bool isBoosting = false;
    private Vector3 normalVector = Vector3.up;

    // Rotation and look
    private float xRotation;
    private float yRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    // Movement
    public float walkSpeed = 7f;
    public float runSpeed = 15f;
    public float boostSpeed = 20f;
    public float maxSpeed = 20f;
    public bool grounded;
    public LayerMask whatIsGround;
    private bool isSliding = false;

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
    float x, y;
    bool jumping, crouching, sprinting;

    // Step Sound
    public AudioClip stepSound;
    private AudioSource audioSource;
    private float stepInterval = 0.5f;
    private float nextStepTime = 0f;
    private bool isPlayingStepSound = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //if (postProcessingVolume.profile.TryGet(out ChromaticAberration ca))
        //    chromaticAberration = ca;
        //if (postProcessingVolume.profile.TryGet(out LensDistortion ld))
        //    lensDistortion = ld;
    }

    private void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCam.gameObject.SetActive(true);
        canvasPause.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        initialCamPosition = playerCam.localPosition;
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 20f;
            Destroy(collision.gameObject);
        }
    }
   

    private Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Sin(u * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private void Update()
    {
        MyInput();
        Look();
        SmoothCameraShake();
        PlayStepSound();
        UpdateUI();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.C);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        if (sprinting )
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



        if (Input.GetKeyDown(KeyCode.C))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.C))
            StopCrouch();
    }

    private void StartCrouch()
    {
        if (sprinting && grounded)
        {
            isSliding = true;
            rb.AddForce(orientation.transform.forward * runSpeed, ForceMode.VelocityChange);
        }
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    private void StopCrouch()
    {
        isSliding = false;
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void StartBoost()
    {
        isBoosting = true;
        rb.drag = 0.5f;
        if (stamina > minStamina)
        {
            stamina -= Time.deltaTime * 10f;
            ApplyCameraEffects(true);
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
        ApplyCameraEffects(false);
    }

    private void ApplyCameraEffects(bool isActive)
    {
        float targetChromaticValue = isActive ? 0.6f : 0f;
        float targetLensDistortionValue = isActive ? -0.3f : 0f;
        // Apply camera effects here (like Chromatic Aberration and Lens Distortion)
    }

    private void Movement()
    {
        float currentSpeed = isBoosting ? boostSpeed : (sprinting ? runSpeed : walkSpeed);
        Vector2 mag = FindVelRelativeToLook();
        CounterMovement(x, y, mag);

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
            rb.AddForce(normalVector * jumpForce * 0.5f);


            Vector3 vel = rb.velocity;
            rb.velocity = new Vector3(vel.x, vel.y > 0 ? vel.y / 2 : 0, vel.z);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);
        yRotation += mouseX;

        playerCam.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f)
            rb.AddForce(orientation.transform.right * -mag.x * counterMovement);
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f)
            rb.AddForce(orientation.transform.forward * -mag.y * counterMovement);

        if (rb.velocity.magnitude > maxSpeed)
        {
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, rb.velocity.y, n.z);
        }
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
        if (grounded && (Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.z) > 0.1f) && !isPlayingStepSound && Time.time >= nextStepTime)
        {
            isPlayingStepSound = true;
            audioSource.PlayOneShot(stepSound);
            nextStepTime = Time.time + stepInterval;
        }
    }

    private void UpdateUI()
    {
        healthText.text = health.ToString();
        staminaBar.fillAmount = stamina / maxStamina;
    }

    private void TogglePause()
    {
        canvasPause.gameObject.SetActive(!canvasPause.gameObject.activeSelf);
        canvasMain.gameObject.SetActive(!canvasMain.gameObject.activeSelf);
        Time.timeScale = canvasPause.gameObject.activeSelf ? 0 : 1;
    }

   
}




