using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName = "Item";
    private static int totalTriggers = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            totalTriggers++;

            Debug.Log($"=== TRIGGER #{totalTriggers} ===");
            Debug.Log($"Item: {itemName}");
            Debug.Log($"Item Instance ID: {GetInstanceID()}");
            Debug.Log($"Triggered by: {other.gameObject.name}");
            Debug.Log($"Time: {Time.time}");
            Debug.Log($"Frame: {Time.frameCount}");

            LevelManager.Instance.CollectItem();
            Destroy(gameObject);
        }
    }
}