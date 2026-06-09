using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"REGULAR OBSTACLE - Resetting level!");

            if (TimeManager.Instance != null)
                TimeManager.Instance.RetryLevel();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}