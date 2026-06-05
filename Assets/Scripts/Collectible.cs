using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName = "Item";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collected: " + itemName);
            LevelManager.Instance.CollectItem();
            Destroy(gameObject);
        }
    }
}