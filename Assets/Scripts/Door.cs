using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ Use the public method, NOT private variable
            int collected = LevelManager.Instance.GetCollectedCount();

            if (collected >= 1)
            {
                Debug.Log("Level Complete! You passed!");
                LevelManager.Instance.ShowLevelComplete();
            }
            else
            {
                Debug.Log($"Need at least 1 item! You have {collected}");
            }
        }
    }
}