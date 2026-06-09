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

    // Per-object flag — tells OnDestroy whether THIS object subscribed to sceneLoaded.
    // Safer than checking Instance==this because Instance can be overwritten before
    // OnDestroy fires (which was exactly what caused "Real instance destroyed" on duplicates).
    private bool isRealInstance = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            isRealInstance = true;
            DontDestroyOnLoad(gameObject);

            // Subscribe in Awake (not Start) so it's set up before any scene logic runs,
            // and is never accidentally removed by a duplicate's OnDestroy.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("=== TimeManager Created & sceneLoaded subscribed ===");
        }
        else
        {
            // isRealInstance stays false — OnDestroy will not touch sceneLoaded.
            Debug.Log("[TimeManager] Duplicate detected — destroying, keeping original.");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Only unsubscribe if THIS object was the one that subscribed.
        // Duplicates never subscribed (isRealInstance=false), so they must never
        // unsubscribe — doing so would remove the real instance's listener.
        if (isRealInstance)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("[TimeManager] Real instance destroyed — unsubscribed sceneLoaded.");
        }
        else
        {
            Debug.Log("[TimeManager] Duplicate destroyed — sceneLoaded untouched.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[TimeManager] Scene loaded: {scene.name} | isTiming={isTiming}");

        // Clear stale UI refs and find new ones in the loaded scene
        timerText = null;
        retryText = null;
        FindTimerUI();

        // Stop timing on non-game scenes
        if (scene.name == "MainMenu" || scene.name == "Finish")
        {
            isTiming = false;
            Debug.Log($"[TimeManager] Stopped timing on {scene.name}");
        }
    }

    void FindTimerUI()
    {
        TimerUI timerUI = FindObjectOfType<TimerUI>();
        if (timerUI != null)
        {
            timerText = timerUI.GetComponent<TextMeshProUGUI>();
            Debug.Log("[TimeManager] TimerUI found and connected");
        }
        else
        {
            Debug.Log("[TimeManager] No TimerUI in this scene (OK for MainMenu/Finish)");
        }

        RetryUI retryUI = FindObjectOfType<RetryUI>();
        if (retryUI != null)
        {
            retryText = retryUI.GetComponent<TextMeshProUGUI>();
            Debug.Log("[TimeManager] RetryUI found and connected");
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
        Debug.Log($"[TimeManager] Starting timer for {levelName}");
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
        Debug.Log($"[TimeManager] RETRY #{retryCount} — timer reset");
    }

    public void CompleteLevel(string levelName)
    {
        isTiming = false;
        Debug.Log($"[TimeManager] Level complete: {levelName} — Time: {FormatTime(currentLevelTime)}");

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
        Debug.Log("[TimeManager] New game — all stats reset");
    }

    public float GetTotalGameTime() => level1FinalTime + level2FinalTime;
    public int GetTotalRetries() => retryCount;
    public float GetLevel1Time() => level1FinalTime;
    public float GetLevel2Time() => level2FinalTime;

    public string GetGrade()
    {
        float effectiveTime = GetTotalGameTime() + retryCount * 2f;

        if (effectiveTime < 30f && retryCount == 0) return "S+ (SUPERB!)";
        else if (effectiveTime < 45f && retryCount <= 1) return "S (EXCELLENT!)";
        else if (effectiveTime < 60f && retryCount <= 2) return "A (GREAT!)";
        else if (effectiveTime < 90f && retryCount <= 3) return "B (GOOD!)";
        else if (effectiveTime < 120f) return "C (PASSING)";
        else return "D (NEED IMPROVEMENT)";
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}