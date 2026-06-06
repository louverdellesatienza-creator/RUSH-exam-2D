using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        // Freeze rotation to prevent spinning
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Debug.Log($"{gameObject.name} is ready! Use WASD or Arrow Keys to move.");
    }

    void Update()
    {
        // === MOVEMENT (WASD or Arrow Keys) ===
        // Horizontal: A/LeftArrow = -1, D/RightArrow = 1
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Vertical: W/UpArrow for jump (handled below)

        // Apply movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // === FLIP SPRITE (face left/right) ===
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // === GROUND CHECK ===
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // === JUMP (W, UpArrow, or Space) ===
        // Check if jump key is pressed AND player is grounded
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            Debug.Log("Jumped!");
        }

        // Debug: Show if grounded
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"Grounded: {isGrounded}");
        }
    }

    // === COLLISION WITH OBSTACLES ===
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object has the "Obstacle" tag
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log($"Hit obstacle: {collision.gameObject.name} - Resetting level!");

            // Increase retry count
            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            // Reset current level
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    // === COLLECTION WITH COLLECTIBLES ===
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object has the "Collectible" tag
        if (other.CompareTag("Collectible"))
        {
            Debug.Log($"Collected: {other.gameObject.name}");

            // Add to LevelManager
            if (LevelManager.Instance != null)
                LevelManager.Instance.CollectItem();

            // Destroy the collectible
            Destroy(other.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        // Visualize ground check in Scene view
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}