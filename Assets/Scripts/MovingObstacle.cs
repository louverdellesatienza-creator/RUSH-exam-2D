using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    public float startDirection = 1f;

    [Header("Ground Check")]
    public bool stayOnGround = true;

    private Vector3 startPosition;
    private float currentDirection;
    private Rigidbody2D rb;
    private bool hasTriggeredReload = false; // ← Prevents double scene reload

    void Start()
    {
        startPosition = transform.position;
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();

        // DIAGNOSTIC: Check all components
        Debug.Log("=== MOVING OBSTACLE DIAGNOSTIC ===");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Debug.Log($"✅ Collider: {col.GetType().Name}");
            Debug.Log($"   Is Trigger: {col.isTrigger}");
            Debug.Log($"   Enabled: {col.enabled}");
        }
        else
        {
            Debug.LogError("❌ NO COLLIDER! Add a BoxCollider2D");
        }

        if (rb != null)
        {
            Debug.Log($"✅ Rigidbody2D: {rb.bodyType}");
            Debug.Log($"   Simulated: {rb.simulated}");
            // FIX: Freeze Y position too so obstacle only moves horizontally
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            Debug.LogError("❌ NO RIGIDBODY2D! Add a Rigidbody2D");
        }

        Debug.Log($"Tag: {gameObject.tag}");

        if (col != null)
            Debug.Log($"Collider size: {col.bounds.size}");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float distanceTravelled = transform.position.x - startPosition.x;

        // Flip direction and sprite when reaching either boundary
        if (distanceTravelled >= moveDistance && currentDirection > 0)
        {
            currentDirection = -1f;
            FlipSprite();
        }
        else if (distanceTravelled <= -moveDistance && currentDirection < 0)
        {
            currentDirection = 1f;
            FlipSprite();
        }

        rb.linearVelocity = new Vector2(currentDirection * moveSpeed, rb.linearVelocity.y);
    }

    void FlipSprite()
    {
        // Flip by inverting the X scale — same technique as PlayerController
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"=== COLLISION DETECTED by Moving Obstacle ===");
        Debug.Log($"Hit: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Player"))
        {
            // FIX: Guard flag so only one script triggers the reload, not both
            if (hasTriggeredReload) return;
            hasTriggeredReload = true;

            Debug.Log($"Player hit moving obstacle: {gameObject.name}");

            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // FIX: Use transform.position as fallback when startPosition is zero (edit mode)
    void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin + Vector3.left * moveDistance, origin + Vector3.right * moveDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}