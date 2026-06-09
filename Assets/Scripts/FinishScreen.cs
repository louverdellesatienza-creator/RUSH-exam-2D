using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FinishScreen : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI level1Text;
    public TextMeshProUGUI level2Text;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI totalItemsText;
    public TextMeshProUGUI retryCountText;
    public TextMeshProUGUI finalGradeText;
    public TextMeshProUGUI scoreMessageText;

    [Header("Buttons")]
    public Button playAgainButton;
    public Button quitButton;

    [Header("Badges")]
    public GameObject perfectBonusBadge;
    public GameObject noRetryBadge;

    void Start()
    {
        DisplayResults();
        SetupButtons();
    }

    void DisplayResults()
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager.Instance is NULL!");
            return;
        }

        // Get saved item counts
        int level1Items = PlayerPrefs.GetInt("Level1Stars", 0);
        int level2Items = PlayerPrefs.GetInt("Level2Stars", 0);
        int totalItems = level1Items + level2Items;
        int maxItems = 10; // 5 + 5

        // Get times
        float level1Time = TimeManager.Instance.GetLevel1Time();
        float level2Time = TimeManager.Instance.GetLevel2Time();
        float totalTime = TimeManager.Instance.GetTotalGameTime();
        int totalRetries = TimeManager.Instance.GetTotalRetries();

        // Display level stats
        if (level1Text != null)
            level1Text.text = $"LEVEL 1: {FormatTime(level1Time)}  |  Items: {level1Items}/5";

        if (level2Text != null)
            level2Text.text = $"LEVEL 2: {FormatTime(level2Time)}  |  Items: {level2Items}/5";

        // Display totals
        if (totalTimeText != null)
            totalTimeText.text = $"TOTAL TIME: {FormatTime(totalTime)}";

        if (totalItemsText != null)
            totalItemsText.text = $"TOTAL ITEMS: {totalItems}/{maxItems}";

        if (retryCountText != null)
            retryCountText.text = $"TOTAL RETRIES: {totalRetries}";

        // Calculate grade
        string grade = GetGrade(totalItems, maxItems, totalRetries);
        if (finalGradeText != null)
            finalGradeText.text = $"FINAL GRADE: {grade}";

        // Display special message
        string message = GetMessage(totalItems, maxItems, totalRetries);
        if (scoreMessageText != null)
            scoreMessageText.text = message;

        // Show badges
        if (perfectBonusBadge != null)
            perfectBonusBadge.SetActive(totalItems == maxItems && totalRetries == 0);

        if (noRetryBadge != null)
            noRetryBadge.SetActive(totalRetries == 0);
    }

    string GetGrade(int items, int maxItems, int retries)
    {
        float percentage = (float)items / maxItems;

        if (percentage >= 0.9f && retries == 0)
            return "S+ (SUPERB!) ⭐⭐⭐⭐⭐";
        else if (percentage >= 0.9f)
            return "S (EXCELLENT!) ⭐⭐⭐⭐";
        else if (percentage >= 0.8f && retries <= 1)
            return "S (EXCELLENT!) ⭐⭐⭐⭐";
        else if (percentage >= 0.7f && retries <= 2)
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

    string GetMessage(int items, int maxItems, int retries)
    {
        float percentage = (float)items / maxItems;

        if (items == maxItems && retries == 0)
            return "PERFECT RUN! YOU'RE A LEGEND!";
        else if (items == maxItems)
            return "PERFECT! ALL ITEMS COLLECTED!";
        else if (percentage >= 0.7f && retries == 0)
            return "GREAT JOB! NO RETRIES!";
        else if (percentage >= 0.7f)
            return "GOOD JOB! KEEP IMPROVING!";
        else if (percentage >= 0.5f)
            return "GOOD START! TRY AGAIN!";
        else
            return "PRACTICE MAKES PERFECT! TRY AGAIN!";
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    void SetupButtons()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void PlayAgain()
    {
        // Reset saved data
        PlayerPrefs.DeleteKey("Level1Stars");
        PlayerPrefs.DeleteKey("Level2Stars");
        PlayerPrefs.Save();

        if (TimeManager.Instance != null)
            TimeManager.Instance.NewGame();

        SceneManager.LoadScene("MainMenu");
    }

    void QuitGame()
    {
        // FIX: Was Application.Quit() — closes the app entirely (does nothing in the Editor).
        // Navigate back to MainMenu instead, which is what the button is supposed to do.
        SceneManager.LoadScene("MainMenu");
    }
}