using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;

    [Header("Air Control")]
    public float airControl = 0.3f;

    [Header("Boundaries")]
    public float leftBoundary = -8f;
    public float rightBoundary = 8f;
    public bool useCameraBounds = true;

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
    private bool hasTriggeredReload = false; // ← Prevents double scene reload

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        mainCamera = Camera.main;
        UpdateCameraBounds();

        Debug.Log($"{gameObject.name} is ready! Boundaries: Left={cameraLeft}, Right={cameraRight}");
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // === GROUND CHECK (do this first so jump check is accurate) ===
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // === HORIZONTAL MOVEMENT ===
        // FIX: Only override X velocity, never touch Y — preserves jump arc cleanly
        float targetX = moveInput * moveSpeed * (isGrounded ? 1f : airControl);
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        // === FLIP SPRITE ===
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // === JUMP ===
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            // FIX: Set Y velocity directly for a snappy, consistent jump height
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // === BETTER FALL PHYSICS ===
        // FIX: Applied via AddForce instead of direct velocity mutation to avoid fighting the engine
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * rb.mass * Time.deltaTime, ForceMode2D.Impulse);
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * rb.mass * Time.deltaTime, ForceMode2D.Impulse);
        }

        // === CONTAIN PLAYER WITHIN BOUNDARIES ===
        ContainPlayer();
    }

    void UpdateCameraBounds()
    {
        if (mainCamera != null)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            cameraLeft = mainCamera.transform.position.x - cameraWidth / 2 + 0.5f;
            cameraRight = mainCamera.transform.position.x + cameraWidth / 2 - 0.5f;
        }
        else
        {
            cameraLeft = leftBoundary;
            cameraRight = rightBoundary;
        }
    }

    void ContainPlayer()
    {
        Vector3 pos = transform.position;

        if (useCameraBounds && mainCamera != null)
            UpdateCameraBounds();

        if (pos.x < cameraLeft)
        {
            pos.x = cameraLeft;
            transform.position = pos;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (pos.x > cameraRight)
        {
            pos.x = cameraRight;
            transform.position = pos;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // FIX: Guard flag so only one script triggers the reload, not both
            if (hasTriggeredReload) return;
            hasTriggeredReload = true;

            Debug.Log($"⚠️ PLAYER CONTROLLER - Hit obstacle: {collision.gameObject.name}");

            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

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