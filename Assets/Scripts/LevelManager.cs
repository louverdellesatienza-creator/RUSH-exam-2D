using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public int totalCollectibles = 3;
    public string nextLevelName = "Level2";  // Change to your next level scene name
    public int minItemsForNextLevel = 3;     // Need all 3 to unlock next level

    [Header("UI References")]
    public TextMeshProUGUI collectibleText;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI ratingText;
    public GameObject star1, star2, star3;
    public Button nextLevelButton;

    [Header("Internal")]
    private int collectedCount = 0;
    private bool levelCompleted = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        levelCompletePanel.SetActive(false);

        // Disable next level button if not all items collected
        if (nextLevelButton != null)
            nextLevelButton.interactable = false;
    }

    public void CollectItem()
    {
        if (levelCompleted) return;

        collectedCount++;
        UpdateUI();

        // Check if all items collected - update next level button
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
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        Time.timeScale = 0f;  // Pause game

        // Show panel
        levelCompletePanel.SetActive(true);

        // Update items text
        if (itemsText != null)
            itemsText.text = $"Items Collected: {collectedCount}/{totalCollectibles}";

        // Update stars based on collected count
        UpdateStars();

        // Update rating text
        UpdateRating();

        // Enable/disable next level button based on collection
        if (nextLevelButton != null)
        {
            bool canGoNext = (collectedCount >= minItemsForNextLevel);
            nextLevelButton.interactable = canGoNext;

            // Change button color if disabled
            var colors = nextLevelButton.colors;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            nextLevelButton.colors = colors;
        }
    }

    void UpdateStars()
    {
        // Show stars based on collected count
        if (star1 != null) star1.SetActive(collectedCount >= 1);
        if (star2 != null) star2.SetActive(collectedCount >= 2);
        if (star3 != null) star3.SetActive(collectedCount >= 3);
    }

    void UpdateRating()
    {
        string rating = "";
        string grade = "";

        switch (collectedCount)
        {
            case 3:
                rating = "⭐⭐⭐ PERFECT!";
                grade = "A+";
                break;
            case 2:
                rating = "⭐⭐ GOOD!";
                grade = "B";
                break;
            case 1:
                rating = "⭐ PASSING";
                grade = "C";
                break;
            default:
                rating = "⭐ NO STARS";
                grade = "F - TRY AGAIN!";
                break;
        }

        if (ratingText != null)
            ratingText.text = $"{rating}\nGrade: {grade}";
    }

    // Button Methods
    public void ReplayLevel()
    {
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
        else
        {
            Debug.Log($"Need {minItemsForNextLevel} items to unlock next level! Currently: {collectedCount}");
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public int GetCollectedCount()
    {
        return collectedCount;
    }
}