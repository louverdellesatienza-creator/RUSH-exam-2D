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
    public GameObject settingsCanvas;  // Drag Canvas_Settings here (starts disabled)

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
        Debug.Log("=== LEVEL MANAGER START ===");

        collectedCount = 0;
        levelCompleted = false;
        UpdateUI();

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.interactable = false;

        // Make sure settings panel starts HIDDEN (not auto-show)
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

        // Apply selected character from SettingsScene
        ApplySelectedCharacter();

        // Start the timer
        if (TimeManager.Instance != null)
            TimeManager.Instance.StartLevel(SceneManager.GetActiveScene().name);

        Debug.Log("Level 1 started - Settings panel will NOT auto-show. Use ⚙️ button to open.");
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
        Time.timeScale = 0f; // Pause game

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(true);
            Debug.Log("Settings panel opened");
        }
        else
        {
            Debug.LogError("Settings Canvas is NULL! Assign it in Inspector.");
        }
    }

    public void CloseSettings()
    {
        Debug.Log("Closing settings panel");
        isSettingsOpen = false;
        Time.timeScale = 1f; // Resume game

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }

        // Re-apply character in case it was changed
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

        // Update stars
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

        if (ratingText != null)
            ratingText.text = rating;

        // Enable/disable next level button
        if (nextLevelButton != null)
            nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);
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