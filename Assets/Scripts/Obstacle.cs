using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();  // ← Changed from AddRetry to RetryLevel

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}