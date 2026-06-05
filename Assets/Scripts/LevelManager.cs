using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int totalCollectibles = 3;
    private int collectedCount = 0;  // This is private (can't be accessed directly)

    public Text collectibleText;
    public GameObject levelCompletePanel;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem()
    {
        collectedCount++;
        UpdateUI();

        Debug.Log($"Collected: {collectedCount}/{totalCollectibles}");

        if (collectedCount >= totalCollectibles)
        {
            Debug.Log("All items collected! Door unlocked!");
        }
    }

    void UpdateUI()
    {
        if (collectibleText != null)
            collectibleText.text = $"Items: {collectedCount}/{totalCollectibles}";
    }

    // ✅ PUBLIC METHOD to get collected count (FIXES your error)
    public int GetCollectedCount()
    {
        return collectedCount;
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }
}