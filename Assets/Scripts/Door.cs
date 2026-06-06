using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Door trigger entered by: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched door! Calling CompleteLevel...");

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.CompleteLevel();
                Debug.Log("CompleteLevel called successfully");
            }
            else
            {
                Debug.LogError("LevelManager.Instance is NULL!");
            }
        }
    }
}