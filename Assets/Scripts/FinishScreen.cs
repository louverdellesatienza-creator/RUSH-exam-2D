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
        float totalTime = TimeManager.Instance.GetTotalTime();
        int totalRetries = TimeManager.Instance.retries;
        float level1Time = TimeManager.Instance.level1Time;
        float level2Time = TimeManager.Instance.level2Time;

        // Display times
        if (totalTimeText != null)
            totalTimeText.text = $"TOTAL TIME: {FormatTime(totalTime)}";

        if (level1TimeText != null)
            level1TimeText.text = $"LEVEL 1: {FormatTime(level1Time)}";

        if (level2TimeText != null)
            level2TimeText.text = $"LEVEL 2: {FormatTime(level2Time)}";

        if (retryCountText != null)
            retryCountText.text = $"TOTAL RETRIES: {totalRetries}";

        // Calculate and display grade
        string grade = TimeManager.Instance.GetGrade();
        if (finalGradeText != null)
            finalGradeText.text = $"FINAL GRADE: {grade}";

        // Display special messages
        string message = "";

        if (totalTime < 30f && totalRetries == 0)
        {
            message = "🏆 PERFECT RUN! YOU'RE A LEGEND! 🏆";
            if (perfectBonusBadge != null) perfectBonusBadge.SetActive(true);
        }
        else if (totalTime < 45f && totalRetries <= 1)
        {
            message = "✨ EXCELLENT! STI PROUD OF YOU! ✨";
        }
        else if (totalTime < 60f)
        {
            message = "👍 GOOD JOB! KEEP IMPROVING! 👍";
        }
        else
        {
            message = "📚 PRACTICE MAKES PERFECT! TRY AGAIN! 📚";
        }

        if (totalRetries == 0)
        {
            message += "\n🎯 PERFECT - NO RETRIES! 🎯";
            if (noRetryBadge != null) noRetryBadge.SetActive(true);
        }

        if (scoreMessageText != null)
            scoreMessageText.text = message;
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public void PlayAgain()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.NewGame();  // ← Changed from ResetForNewGame to NewGame

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}