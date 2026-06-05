using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call LevelManager to show completion screen
            LevelManager.Instance.CompleteLevel();
        }
    }
}