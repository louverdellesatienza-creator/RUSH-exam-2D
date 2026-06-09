using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public int totalCollectibles = 5;
    public string nextLevelName = "Level2";
    public int minItemsForNextLevel = 3;

    [Header("UI References")]
    public TextMeshProUGUI collectibleText;
    public GameObject canvas_LevelComplete;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI timeText;
    public GameObject star1, star2, star3, star4, star5;
    public Button nextLevelButton;
    public Button replayButton;
    public Button quitButton;

    [Header("Color Settings")]
    public Color passingColor = Color.green;
    public Color notPassingColor = Color.red;
    public Color defaultColor = Color.white;

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
        // FIX 1: Always force timeScale back to 1 the moment this scene loads.
        // CompleteLevel() and OpenSettings() both set timeScale=0 but scene loading
        // does NOT reset it — so Level1 loads frozen if you came from a completed/paused state.
        Time.timeScale = 1f;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        collectedCount = 0;
        levelCompleted = false;

        Debug.Log($"=== LEVEL MANAGER START - {SceneManager.GetActiveScene().name} ===");
        Debug.Log($"Time.timeScale on Start = {Time.timeScale}"); // Should always be 1 now

        StartCoroutine(DelayedStart());
    }

    System.Collections.IEnumerator DelayedStart()
    {
        yield return null; // Wait one frame for scene to fully settle

        UpdateUI();

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.interactable = false;

        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

        if (notEnoughItemsMessage != null)
            notEnoughItemsMessage.SetActive(false);

        ApplySelectedCharacter();
        SetLevelCollectibles();

        // FIX 2: Removed the 30-frame spin-wait for TimeManager.
        // If TimeManager uses DontDestroyOnLoad it's already alive in Awake/Start order.
        // If it doesn't exist yet, waiting 30 frames just delays the start visibly (the "stuck" feeling).
        // Instead: try immediately, and if null, log a clear error so you know to fix the setup.
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StartLevel(SceneManager.GetActiveScene().name);
            Debug.Log($"✅ Level started — need {minItemsForNextLevel}/{totalCollectibles} items to pass");
        }
        else
        {
            // TimeManager is missing entirely — log exactly where to fix it
            Debug.LogError(
                "❌ TimeManager.Instance is NULL when Level started!\n" +
                "Fix: Make sure TimeManager is in your MainMenu scene (or an earlier scene) " +
                "with DontDestroyOnLoad, so it persists into Level1.\n" +
                "The level will still run, but timing won't be tracked."
            );
        }
    }

    void SetLevelCollectibles()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level1" || sceneName == "Level1-outside school")
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
            settingsCanvas.SetActive(true);
        else
            Debug.LogError("Settings Canvas is NULL!");
    }

    public void CloseSettings()
    {
        Debug.Log("Closing settings panel");
        isSettingsOpen = false;
        Time.timeScale = 1f;

        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

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

    // FIX 3: Removed OnEnable() resetting collectedCount and levelCompleted.
    // OnEnable fires BEFORE Start() when the scene loads — so it was resetting
    // values that Start() had just initialized, and also firing on every
    // SetActive(true) call on this GameObject mid-game, silently wiping progress.
    // Start() already handles initialization; OnEnable() was redundant and harmful.

    void UpdateUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"Items: {collectedCount}/{totalCollectibles}";
            collectibleText.color = collectedCount >= minItemsForNextLevel ? passingColor
                                  : collectedCount > 0 ? notPassingColor
                                                                           : defaultColor;
        }

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

        if (collectedCount < minItemsForNextLevel)
        {
            Debug.Log($"Cannot complete level! Need {minItemsForNextLevel} items, only have {collectedCount}");
            ShowNotEnoughItemsMessage();
            return;
        }

        levelCompleted = true;

        if (TimeManager.Instance != null)
            TimeManager.Instance.CompleteLevel(SceneManager.GetActiveScene().name);

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

        if (ratingText != null)
            ratingText.text = GetRatingText();

        if (nextLevelButton != null)
            nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);

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
        return collectedCount switch
        {
            5 => "PERFECT! (A+)",
            4 => "GREAT! (A)",
            3 => "GOOD! (B)",
            2 => "PASSING (C)",
            _ => "NEED IMPROVEMENT (F)"
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