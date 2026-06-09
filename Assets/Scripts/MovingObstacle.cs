using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;           // Speed of movement
    public float moveDistance = 3f;        // How far it moves left/right
    public float startDirection = 1f;      // 1 = right, -1 = left

    [Header("Ground Check")]
    public bool stayOnGround = true;       // Keep obstacle on ground

    private Vector3 startPosition;
    private float currentDirection;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();

        // Freeze rotation to prevent spinning
        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Move the obstacle
        float move = currentDirection * moveSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        // Check if reached the right boundary
        if (transform.position.x >= startPosition.x + moveDistance)
        {
            currentDirection = -1f;  // Turn left
            FlipSprite();
        }
        // Check if reached the left boundary
        else if (transform.position.x <= startPosition.x - moveDistance)
        {
            currentDirection = 1f;   // Turn right
            FlipSprite();
        }

        // Keep on ground if needed
        if (stayOnGround)
        {
            Vector3 pos = transform.position;
            pos.y = startPosition.y;
            transform.position = pos;
        }
    }

    void FlipSprite()
    {
        // Flip the sprite to face movement direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (currentDirection > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If player hits this obstacle, reset level
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player hit moving obstacle: {gameObject.name}");

            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    // Visualize movement range in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 leftBound = startPosition + Vector3.left * moveDistance;
        Vector3 rightBound = startPosition + Vector3.right * moveDistance;
        Gizmos.DrawLine(leftBound, rightBound);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}