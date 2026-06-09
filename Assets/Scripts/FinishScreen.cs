using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishScreen : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI level1TimeText;
    public TextMeshProUGUI level2TimeText;
    public TextMeshProUGUI retryCountText;
    public TextMeshProUGUI finalGradeText;
    public TextMeshProUGUI scoreMessageText;

    [Header("Item Display")]
    public TextMeshProUGUI level1ItemsText;
    public TextMeshProUGUI level2ItemsText;
    public TextMeshProUGUI totalItemsText;

    public GameObject perfectBonusBadge;
    public GameObject noRetryBadge;

    void Start()
    {
        DisplayResults();
    }

    void DisplayResults()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager.Instance is NULL!");
            return;
        }

        // Get times from TimeManager
        float totalTime = TimeManager.Instance.GetTotalGameTime();
        int totalRetries = TimeManager.Instance.GetTotalRetries();
        float level1Time = TimeManager.Instance.GetLevel1Time();
        float level2Time = TimeManager.Instance.GetLevel2Time();

        // GET COLLECTED ITEMS FROM PLAYER PREFS
        int level1Collected = PlayerPrefs.GetInt("Level1Stars", 0);
        int level1Total = 5;  // Level 1 has 5 items
        int level2Collected = PlayerPrefs.GetInt("Level2Stars", 0);
        int level2Total = 5;  // Level 2 has 5 items

        int totalCollected = level1Collected + level2Collected;
        int totalPossible = level1Total + level2Total;

        // Display times
        if (totalTimeText != null)
            totalTimeText.text = $"TOTAL TIME: {FormatTime(totalTime)}";

        if (level1TimeText != null)
            level1TimeText.text = $"LEVEL 1: {FormatTime(level1Time)}";

        if (level2TimeText != null)
            level2TimeText.text = $"LEVEL 2: {FormatTime(level2Time)}";

        if (retryCountText != null)
            retryCountText.text = $"TOTAL RETRIES: {totalRetries}";

        // DISPLAY ITEMS COLLECTED
        if (level1ItemsText != null)
            level1ItemsText.text = $"ITEMS: {level1Collected}/{level1Total}";

        if (level2ItemsText != null)
            level2ItemsText.text = $"ITEMS: {level2Collected}/{level2Total}";

        if (totalItemsText != null)
            totalItemsText.text = $"TOTAL ITEMS: {totalCollected}/{totalPossible}";

        // Calculate and display grade based on ITEMS (not time)
        string grade = GetGradeFromItems(level1Collected, level2Collected, totalRetries);
        if (finalGradeText != null)
            finalGradeText.text = $"FINAL GRADE: {grade}";

        // Display special messages
        string message = "";

        float itemPercentage = (float)totalCollected / totalPossible;

        if (totalCollected == totalPossible && totalRetries == 0)
        {
            message = "🏆 PERFECT RUN! ALL ITEMS COLLECTED! NO RETRIES! 🏆";
            if (perfectBonusBadge != null) perfectBonusBadge.SetActive(true);
        }
        else if (totalCollected == totalPossible)
        {
            message = "✨ PERFECT! ALL ITEMS COLLECTED! STI PROUD OF YOU! ✨";
        }
        else if (itemPercentage >= 0.7f)
        {
            message = "👍 GOOD JOB! YOU'RE DOING GREAT! 👍";
        }
        else if (itemPercentage >= 0.5f)
        {
            message = "📚 GOOD START! KEEP PRACTICING! 📚";
        }
        else
        {
            message = "📖 TRY AGAIN! COLLECT MORE ITEMS NEXT TIME! 📖";
        }

        if (totalRetries == 0 && totalCollected < totalPossible)
        {
            message += "\n🎯 NO RETRIES! TRY TO COLLECT MORE ITEMS! 🎯";
            if (noRetryBadge != null) noRetryBadge.SetActive(true);
        }
        else if (totalRetries == 0)
        {
            message += "\n🎯 PERFECT - NO RETRIES! 🎯";
            if (noRetryBadge != null) noRetryBadge.SetActive(true);
        }

        if (scoreMessageText != null)
            scoreMessageText.text = message;
    }

    string GetGradeFromItems(int level1Items, int level2Items, int retries)
    {
        int totalItems = level1Items + level2Items;
        int maxItems = 10;  // 5 + 5
        float percentage = (float)totalItems / maxItems;

        // Retry penalty: subtract 0.5 from grade per retry (max 2 retries count)
        int retryPenalty = Mathf.Min(retries, 3);

        if (percentage >= 0.9f && retryPenalty == 0)
            return "S+ (SUPERB!) ⭐⭐⭐⭐⭐";
        else if (percentage >= 0.9f)
            return "S (EXCELLENT!) ⭐⭐⭐⭐";
        else if (percentage >= 0.8f && retryPenalty <= 1)
            return "S (EXCELLENT!) ⭐⭐⭐⭐";
        else if (percentage >= 0.7f && retryPenalty <= 2)
            return "A (GREAT!) ⭐⭐⭐";
        else if (percentage >= 0.7f)
            return "B (GOOD!) ⭐⭐";
        else if (percentage >= 0.6f)
            return "B (GOOD!) ⭐⭐";
        else if (percentage >= 0.5f)
            return "C (PASSING) ⭐";
        else
            return "D (NEED IMPROVEMENT)";
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public void PlayAgain()
    {
        // Clear saved item data
        PlayerPrefs.DeleteKey("Level1Stars");
        PlayerPrefs.DeleteKey("Level2Stars");
        PlayerPrefs.Save();

        if (TimeManager.Instance != null)
            TimeManager.Instance.NewGame();

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}