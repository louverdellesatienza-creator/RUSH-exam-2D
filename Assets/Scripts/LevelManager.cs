using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public int totalCollectibles = 5;  // 5 items per level
    public string nextLevelName = "Level2";
    public int minItemsForNextLevel = 3;  // Only need 3 to pass

    [Header("UI References")]
    public TextMeshProUGUI collectibleText;
    public GameObject canvas_LevelComplete;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI timeText;
    public GameObject star1, star2, star3, star4, star5;  // 5 stars for 5 items
    public Button nextLevelButton;
    public Button replayButton;
    public Button quitButton;

    [Header("Color Settings")]
    public Color passingColor = Color.green;      // Color when 3+ items
    public Color notPassingColor = Color.red;     // Color when 0-2 items
    public Color defaultColor = Color.white;      // Default color

    [Header("Player References")]
    public GameObject playerBoy;
    public GameObject playerGirl;

    [Header("Settings Panel")]
    public GameObject settingsCanvas;

    [Header("Message")]
    public GameObject notEnoughItemsMessage;

    private int collectedCount = 0;
    private bool levelCompleted = false;
    private bool isSettingsOpen = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // ⭐ CRITICAL: Reset for each new level
        collectedCount = 0;
        levelCompleted = false;

        Debug.Log($"=== LEVEL MANAGER START - {SceneManager.GetActiveScene().name} ===");

        UpdateUI();

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.interactable = false;

        // Make sure settings panel starts HIDDEN
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

        // Hide message if exists
        if (notEnoughItemsMessage != null)
            notEnoughItemsMessage.SetActive(false);

        // Apply selected character
        ApplySelectedCharacter();

        // Set total collectibles based on level
        SetLevelCollectibles();

        // Start the timer
        if (TimeManager.Instance != null)
            TimeManager.Instance.StartLevel(SceneManager.GetActiveScene().name);

        Debug.Log($"Level started - Need to collect {totalCollectibles} items, {minItemsForNextLevel} required to pass");
    }

    void SetLevelCollectibles()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level1-outside school" || sceneName == "Level1")
        {
            totalCollectibles = 5;
            minItemsForNextLevel = 3;
            Debug.Log("Level 1: 5 total items, need 3 to pass");
        }
        else if (sceneName == "Level2")
        {
            totalCollectibles = 5;
            minItemsForNextLevel = 3;
            Debug.Log("Level 2: 5 total items, need 3 to pass");
        }
    }

    void ApplySelectedCharacter()
    {
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");
        Debug.Log($"Applying character: {selectedCharacter}");

        if (playerBoy != null)
            playerBoy.SetActive(selectedCharacter == "Boy");

        if (playerGirl != null)
            playerGirl.SetActive(selectedCharacter == "Girl");
    }

    public void OpenSettings()
    {
        if (levelCompleted) return;

        Debug.Log("Opening settings panel");
        isSettingsOpen = true;
        Time.timeScale = 0f;

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Settings Canvas is NULL!");
        }
    }

    public void CloseSettings()
    {
        Debug.Log("Closing settings panel");
        isSettingsOpen = false;
        Time.timeScale = 1f;

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }

        ApplySelectedCharacter();
    }

    public void CollectItem()
    {
        if (levelCompleted) return;

        collectedCount++;
        UpdateUI();

        Debug.Log($"Collected: {collectedCount}/{totalCollectibles}");

        if (collectedCount >= totalCollectibles)
        {
            if (nextLevelButton != null)
                nextLevelButton.interactable = true;
            Debug.Log("All items collected! Door unlocked!");
        }
    }

    void OnEnable()
    {
        // Reset when scene loads
        collectedCount = 0;
        levelCompleted = false;
    }

    void UpdateUI()
    {
        // Update text display (X/5 format)
        if (collectibleText != null)
        {
            collectibleText.text = $"Items: {collectedCount}/{totalCollectibles}";

            // Change color based on collected count
            if (collectedCount >= minItemsForNextLevel)
            {
                // Green - enough items to pass
                collectibleText.color = passingColor;
            }
            else if (collectedCount > 0)
            {
                // Red - not enough items yet
                collectibleText.color = notPassingColor;
            }
            else
            {
                // White - no items collected
                collectibleText.color = defaultColor;
            }
        }

        // Update 5 stars
        if (star1 != null) star1.SetActive(collectedCount >= 1);
        if (star2 != null) star2.SetActive(collectedCount >= 2);
        if (star3 != null) star3.SetActive(collectedCount >= 3);
        if (star4 != null) star4.SetActive(collectedCount >= 4);
        if (star5 != null) star5.SetActive(collectedCount >= 5);
    }

    public void ShowNotEnoughItemsMessage()
    {
        if (notEnoughItemsMessage != null)
        {
            notEnoughItemsMessage.SetActive(true);
            Invoke("HideMessage", 2f);
        }
        Debug.Log($"Need {minItemsForNextLevel} items, but only have {collectedCount}");
    }

    void HideMessage()
    {
        if (notEnoughItemsMessage != null)
            notEnoughItemsMessage.SetActive(false);
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        // Check if player has enough items to complete
        if (collectedCount < minItemsForNextLevel)
        {
            Debug.Log($"Cannot complete level! Need {minItemsForNextLevel} items, only have {collectedCount}");
            ShowNotEnoughItemsMessage();
            return;
        }

        levelCompleted = true;

        if (TimeManager.Instance != null)
            TimeManager.Instance.CompleteLevel(SceneManager.GetActiveScene().name);

        // SAVE RATING FOR FINISH SCREEN
        SaveLevelRating();

        Time.timeScale = 0f;

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(true);

        if (itemsText != null)
            itemsText.text = $"Items: {collectedCount}/{totalCollectibles}";

        if (timeText != null && TimeManager.Instance != null)
        {
            float t = TimeManager.Instance.currentLevelTime;
            int minutes = Mathf.FloorToInt(t / 60);
            int seconds = Mathf.FloorToInt(t % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        // Update rating based on collected items
        string rating = GetRatingText();

        if (ratingText != null)
            ratingText.text = rating;

        // Enable next level button if enough items collected
        if (nextLevelButton != null)
            nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);

        // Check if this is Level 2 (final level)
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level2")
        {
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(GoToFinish);
                nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);
            }
        }
    }

    string GetRatingText()
    {
        // For 5 items per level
        return collectedCount switch
        {
            5 => "⭐⭐⭐⭐⭐ PERFECT! (A+)",
            4 => "⭐⭐⭐⭐ GREAT! (A)",
            3 => "⭐⭐⭐ GOOD! (B)",
            2 => "⭐⭐ PASSING (C)",
            _ => "⭐ NEED IMPROVEMENT (F)"
        };
    }

    void SaveLevelRating()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int itemsCollected = collectedCount;

        if (sceneName.Contains("Level1"))
        {
            PlayerPrefs.SetInt("Level1Stars", itemsCollected);
            Debug.Log($"Saved Level 1: {itemsCollected}/5 items");
        }
        else if (sceneName.Contains("Level2"))
        {
            PlayerPrefs.SetInt("Level2Stars", itemsCollected);
            Debug.Log($"Saved Level 2: {itemsCollected}/5 items");
        }

        PlayerPrefs.Save();
    }

    void GoToFinish()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Finish");
    }

    public void ReplayLevel()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.RetryLevel();

        collectedCount = 0;
        levelCompleted = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if (collectedCount >= minItemsForNextLevel)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public int GetCollectedCount()
    {
        return collectedCount;
    }
}