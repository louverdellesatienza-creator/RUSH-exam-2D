using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    public AudioClip hitSound;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Play hit sound
            if (hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, transform.position);

            // Reset level on obstacle hit
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}