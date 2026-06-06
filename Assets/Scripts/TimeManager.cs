using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Timing")]
    public float currentLevelTime = 0f;
    public float level1FinalTime = 0f;
    public float level2FinalTime = 0f;
    public int retryCount = 0;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI retryText;

    [Header("Status")]
    public bool isTiming = false;

    private string currentLevel = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("=== TimeManager Created ===");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Find timer UI in the new scene
        if (timerText == null)
        {
            timerText = FindObjectOfType<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (isTiming)
        {
            currentLevelTime += Time.deltaTime;
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (timerText != null)
            timerText.text = $"TIME: {FormatTime(currentLevelTime)}";

        if (retryText != null)
            retryText.text = $"RETRIES: {retryCount}";
    }

    public void StartLevel(string levelName)
    {
        Debug.Log($"=== STARTING TIMER for {levelName} ===");
        currentLevel = levelName;
        currentLevelTime = 0f;
        isTiming = true;
        UpdateDisplay();
    }

    public void RetryLevel()
    {
        retryCount++;
        currentLevelTime = 0f;
        isTiming = true;
        UpdateDisplay();
        Debug.Log($"⚠️ RETRY #{retryCount} - Timer reset to 0");
    }

    public void CompleteLevel(string levelName)
    {
        isTiming = false;
        Debug.Log($"✅ Level Complete: {levelName} - Time: {FormatTime(currentLevelTime)}");

        if (levelName.Contains("Level1"))
            level1FinalTime = currentLevelTime;
        else if (levelName.Contains("Level2"))
            level2FinalTime = currentLevelTime;
    }

    public void NewGame()
    {
        retryCount = 0;
        level1FinalTime = 0f;
        level2FinalTime = 0f;
        currentLevelTime = 0f;
        isTiming = false;
        Debug.Log("🔄 New Game - All stats reset");
    }

    public float GetTotalGameTime()
    {
        return level1FinalTime + level2FinalTime;
    }

    public int GetTotalRetries()
    {
        return retryCount;
    }

    public float GetLevel1Time()
    {
        return level1FinalTime;
    }

    public float GetLevel2Time()
    {
        return level2FinalTime;
    }

    public string GetGrade()
    {
        float totalTime = GetTotalGameTime();
        float penaltyTime = retryCount * 2f;
        float effectiveTime = totalTime + penaltyTime;

        if (effectiveTime < 30f && retryCount == 0)
            return "S+ (SUPERB!) ⭐⭐⭐⭐⭐";
        else if (effectiveTime < 45f && retryCount <= 1)
            return "S (EXCELLENT!) ⭐⭐⭐⭐";
        else if (effectiveTime < 60f && retryCount <= 2)
            return "A (GREAT!) ⭐⭐⭐";
        else if (effectiveTime < 90f && retryCount <= 3)
            return "B (GOOD!) ⭐⭐";
        else if (effectiveTime < 120f)
            return "C (PASSING) ⭐";
        else
            return "D (NEED IMPROVEMENT)";
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}