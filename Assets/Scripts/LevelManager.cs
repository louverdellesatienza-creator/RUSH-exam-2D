using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public int totalCollectibles = 3;
    public string nextLevelName = "Level2";
    public int minItemsForNextLevel = 3;

    [Header("UI References")]
    public TextMeshProUGUI collectibleText;
    public GameObject canvas_LevelComplete;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI timeText;
    public GameObject star1, star2, star3;
    public Button nextLevelButton;
    public Button replayButton;
    public Button quitButton;

    [Header("Player References")]
    public GameObject playerBoy;
    public GameObject playerGirl;

    [Header("Settings Panel")]
    public GameObject settingsCanvas;

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
        Debug.Log($"=== LEVEL MANAGER START - {SceneManager.GetActiveScene().name} ===");

        collectedCount = 0;
        levelCompleted = false;
        UpdateUI();

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.interactable = false;

        // Make sure settings panel starts HIDDEN
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

        // Apply selected character
        ApplySelectedCharacter();

        // Set total collectibles based on level
        SetLevelCollectibles();

        // Start the timer
        if (TimeManager.Instance != null)
            TimeManager.Instance.StartLevel(SceneManager.GetActiveScene().name);

        Debug.Log($"Level started - Need to collect {totalCollectibles} items");
    }

    void SetLevelCollectibles()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level1-outside school" || sceneName == "Level1")
        {
            totalCollectibles = 3;
            minItemsForNextLevel = 3;
            Debug.Log("Level 1: Need 3 items to unlock next level");
        }
        else if (sceneName == "Level2")
        {
            totalCollectibles = 5;
            minItemsForNextLevel = 3;
            Debug.Log("Level 2: Need 5 items for perfect, 3 to pass");
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

    void UpdateUI()
    {
        if (collectibleText != null)
            collectibleText.text = $"Items: {collectedCount}/{totalCollectibles}";

        if (star1 != null) star1.SetActive(collectedCount >= 1);
        if (star2 != null) star2.SetActive(collectedCount >= 2);
        if (star3 != null) star3.SetActive(collectedCount >= 3);
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        levelCompleted = true;

        if (TimeManager.Instance != null)
            TimeManager.Instance.CompleteLevel(SceneManager.GetActiveScene().name);

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
        string rating = collectedCount switch
        {
            3 => "⭐⭐⭐ PERFECT! (A+)",
            2 => "⭐⭐ GOOD! (B)",
            1 => "⭐ PASSING (C)",
            _ => "NO STARS (F)"
        };

        // For Level 2 with 5 items
        if (totalCollectibles == 5)
        {
            rating = collectedCount switch
            {
                5 => "⭐⭐⭐⭐⭐ PERFECT! (A+)",
                4 => "⭐⭐⭐⭐ GREAT! (A)",
                3 => "⭐⭐⭐ GOOD! (B)",
                2 => "⭐⭐ PASSING (C)",
                _ => "⭐ NEED IMPROVEMENT (F)"
            };
        }

        if (ratingText != null)
            ratingText.text = rating;

        // Enable next level button if enough items collected
        if (nextLevelButton != null)
            nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);

        // Check if this is Level 2 (final level)
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level2")
        {
            // Change Next Level button to go to Finish Screen
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(GoToFinish);
                nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);
            }
        }
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