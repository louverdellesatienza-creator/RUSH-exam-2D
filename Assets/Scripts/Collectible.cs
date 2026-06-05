using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName = "ID Card";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ Call with NO arguments
            LevelManager.Instance.CollectItem();
            Destroy(gameObject);
            Debug.Log("Collected: " + itemName);
        }
    }
}