using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    private bool isUnlocked = false;
    public GameObject lockedMessage;
    public Text lockedText;

    void Start()
    {
        lockedMessage.SetActive(false);
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        // Optional: Change door color or add particles
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isUnlocked || LevelManager.Instance.CanCompleteLevel())
            {
                LevelManager.Instance.CompleteLevel();
            }
            else
            {
                // Show locked message
                StartCoroutine(ShowLockedMessage());
            }
        }
    }

    System.Collections.IEnumerator ShowLockedMessage()
    {
        lockedText.text = $"Need {LevelManager.Instance.minRequiredToPass} item! Found: {LevelManager.Instance.collectedCount}";
        lockedMessage.SetActive(true);
        yield return new WaitForSeconds(2f);
        lockedMessage.SetActive(false);
    }
}