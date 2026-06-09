using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;

    [Header("Air Control")]
    public float airControl = 0.3f;

    [Header("Boundaries")]
    public float leftBoundary = -8f;   // Left edge
    public float rightBoundary = 8f;   // Right edge
    public bool useCameraBounds = true; // Auto-detect camera bounds

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    [Header("Fall Physics")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 originalScale;
    private Camera mainCamera;
    private float cameraLeft;
    private float cameraRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Get camera boundaries
        mainCamera = Camera.main;
        UpdateCameraBounds();

        Debug.Log($"{gameObject.name} is ready! Boundaries: Left={cameraLeft}, Right={cameraRight}");
    }

    void Update()
    {
        // === MOVEMENT ===
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed * airControl, rb.linearVelocity.y);
        }

        // === FLIP SPRITE ===
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // === GROUND CHECK ===
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // === JUMP ===
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // === BETTER FALL PHYSICS ===
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // === CONTAIN PLAYER WITHIN BOUNDARIES ===
        ContainPlayer();
    }

    void UpdateCameraBounds()
    {
        if (mainCamera != null)
        {
            // Calculate camera edges in world space
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            cameraLeft = mainCamera.transform.position.x - cameraWidth / 2;
            cameraRight = mainCamera.transform.position.x + cameraWidth / 2;

            // Add small padding (optional)
            cameraLeft += 0.5f;
            cameraRight -= 0.5f;
        }
        else
        {
            // Fallback manual boundaries
            cameraLeft = leftBoundary;
            cameraRight = rightBoundary;
        }
    }

    void ContainPlayer()
    {
        Vector3 pos = transform.position;

        // Update camera bounds in case camera moves
        if (useCameraBounds && mainCamera != null)
        {
            UpdateCameraBounds();
        }

        // Check left boundary
        if (pos.x < cameraLeft)
        {
            pos.x = cameraLeft;
            transform.position = pos;

            // Stop horizontal movement when hitting boundary
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Check right boundary
        if (pos.x > cameraRight)
        {
            pos.x = cameraRight;
            transform.position = pos;

            // Stop horizontal movement when hitting boundary
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log($"Hit obstacle: {collision.gameObject.name} - Resetting level!");

            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            Debug.Log("Player touched door!");
            if (LevelManager.Instance != null)
                LevelManager.Instance.CompleteLevel();
        }
    }

    void OnDrawGizmos()
    {
        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Visualize boundaries
        Gizmos.color = Color.yellow;

        if (mainCamera != null && useCameraBounds)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            float left = mainCamera.transform.position.x - cameraWidth / 2 + 0.5f;
            float right = mainCamera.transform.position.x + cameraWidth / 2 - 0.5f;

            Gizmos.DrawLine(new Vector3(left, -10, 0), new Vector3(left, 10, 0));
            Gizmos.DrawLine(new Vector3(right, -10, 0), new Vector3(right, 10, 0));
        }
        else
        {
            Gizmos.DrawLine(new Vector3(leftBoundary, -10, 0), new Vector3(leftBoundary, 10, 0));
            Gizmos.DrawLine(new Vector3(rightBoundary, -10, 0), new Vector3(rightBoundary, 10, 0));
        }
    }
}