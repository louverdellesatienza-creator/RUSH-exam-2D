using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit obstacle! Resetting level...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}