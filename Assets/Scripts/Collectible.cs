using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum ItemType { IDCard, ExamPermit, Ballpen }
    public ItemType itemType;

    public AudioClip collectSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play sound
            if (collectSound != null)
                audioSource.PlayOneShot(collectSound);

            // Notify LevelManager
            LevelManager.Instance.CollectItem(itemType);

            // Visual feedback (optional)
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            // Destroy after sound (0.2s delay)
            Destroy(gameObject, 0.2f);
        }
    }
}