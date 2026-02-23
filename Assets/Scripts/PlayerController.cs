using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f; // Base walking speed (units per second).
    [SerializeField] private float sprintMultiplier = 1.8f; // Multiplier applied to moveSpeed whil

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 100f; // Mouse sensitivity for look rotation.

    [Header("Sprint Settings")]
    [SerializeField] private float maxStamina = 5f; // Maximum stamina value.
    [SerializeField] private float staminaDrainRate = 1f; // How many stamina units are drained per second while sprinting.
    [SerializeField] private float staminaRegenRate = 1.5f; // How many stamina units are regenerated per second when not sprinting.

    private float currentStamina; // Current stamina amount (0..maxStamina).
    private float xRotation = 0f; // Vertical camera rotation accumulator (in degrees).

    private Rigidbody rb; // Cached Rigidbody reference for physics-based movement.
    private bool isSprinting; // Whether the player is currently sprinting.
    private bool sprintLocked;

    private void Start()
    {
        // Cache the Rigidbody component for use in FixedUpdate movement.
        rb = GetComponent<Rigidbody>();

        // Start with full stamina.
        currentStamina = maxStamina;

        // Lock and hide the cursor for a typical first-person experience.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseLook();     // Rotate camera and player based on mouse input.
        HandleSprint();       // Read sprint input and adjust stamina.
        RegenerateStamina();  // Regenerate stamina when not sprinting.
    }

    private void FixedUpdate()
    {
        HandleMovement(); // Move the Rigidbody according to input and current speed.
    }

    /// <summary>
    /// Read movement input and move the Rigidbody accordingly.
    /// Uses transform.right/forward to move relative to player orientation.
    /// </summary>
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or left/right.
        float moveZ = Input.GetAxis("Vertical");   // W/S or forward/back.

        // Determine movement speed and apply sprint multiplier if applicable.
        float speed = moveSpeed;
        if (isSprinting)
            speed *= sprintMultiplier;

        // Construct movement vector in world space based on local axes.
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Move the Rigidbody to the new position using fixedDeltaTime for consistent physics.
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Handle first-person mouse look. Vertical rotation is clamped to avoid flipping.
    /// </summary>
    private void HandleMouseLook()
    {
        // Read raw mouse input scaled by sensitivity and frame time.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Accumulate vertical rotation and clamp to prevent extreme angles.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Apply vertical rotation to the main camera (pitch).
        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Apply horizontal rotation to the player transform (yaw).
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Process sprint input. When LeftShift is held and stamina remains, enable sprinting and drain stamina.
    /// Otherwise disable sprinting. Clamp stamina to the valid range after modifications.
    /// </summary>
    private void HandleSprint()
    {
        // If sprint is locked, do not allow sprinting
        if (sprintLocked)
        {
            isSprinting = false;
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;

            // If stamina reaches zero, lock sprint
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                sprintLocked = true;
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
        }
    }

    /// <summary>
    /// Regenerate stamina over time when the player is not sprinting and stamina is not full.
    /// </summary>
    private void RegenerateStamina()
    {
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

            // Unlock sprint ONLY when stamina is fully restored
            if (currentStamina >= maxStamina)
            {
                sprintLocked = false;
            }
        }
    }

    public float StaminaNormalized
    {
        get { return currentStamina / maxStamina; } // Return stamina as a normalized value (0..1) for UI or other systems.
    }
}
