using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int totalCollectibles = 3;
    public string nextLevelName = "Level2";
    public int minItemsForNextLevel = 3;

    public TextMeshProUGUI collectibleText;
    public GameObject canvas_LevelComplete;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI timeText;
    public GameObject star1, star2, star3;
    public Button nextLevelButton;

    private int collectedCount = 0;
    private bool levelCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        collectedCount = 0;
        levelCompleted = false;
        UpdateUI();

        if (canvas_LevelComplete != null)
            canvas_LevelComplete.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.interactable = false;

        // START THE TIMER!
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StartLevel();
            Debug.Log("LevelManager: Timer started");
        }
        else
        {
            Debug.LogError("TimeManager.Instance is NULL!");
        }
    }

    public void CollectItem()
    {
        if (levelCompleted) return;

        collectedCount++;
        UpdateUI();

        if (collectedCount >= totalCollectibles && nextLevelButton != null)
            nextLevelButton.interactable = true;
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
            float time = TimeManager.Instance.currentTime;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        string rating = collectedCount switch
        {
            3 => "PERFECT! (A+)",
            2 => "GOOD! (B)",
            1 => "PASSING (C)",
            _ => "NO STARS (F)"
        };

        if (ratingText != null) ratingText.text = rating;

        if (nextLevelButton != null)
            nextLevelButton.interactable = (collectedCount >= minItemsForNextLevel);
    }

    public void ReplayLevel()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.AddRetry();

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
}