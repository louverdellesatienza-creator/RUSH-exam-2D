using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    // Timer values
    public float currentTime = 0f;
    public float level1Time = 0f;
    public float level2Time = 0f;
    public int retries = 0;
    public bool isPlaying = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TimeManager Created!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Only count time if game is playing
        if (isPlaying)
        {
            currentTime += Time.deltaTime;
        }
    }

    // Call this when a level starts
    public void StartLevel()
    {
        currentTime = 0f;
        isPlaying = true;
        Debug.Log("Timer STARTED!");
    }

    // Call this when player dies
    public void AddRetry()
    {
        retries++;
        currentTime = 0f;
        isPlaying = true;
        Debug.Log($"RETRY #{retries} - Timer reset");
    }

    // Call this when level is completed
    public void CompleteLevel(string levelName)
    {
        isPlaying = false;
        Debug.Log($"Level COMPLETE! Time: {currentTime:F2} seconds");

        if (levelName.Contains("Level1"))
            level1Time = currentTime;
        else if (levelName.Contains("Level2"))
            level2Time = currentTime;
    }

    // Call this when starting a new game from menu
    public void NewGame()
    {
        retries = 0;
        level1Time = 0f;
        level2Time = 0f;
        currentTime = 0f;
        isPlaying = false;
        Debug.Log("NEW GAME - All stats reset");
    }

    // Get total time for finish screen
    public float GetTotalTime()
    {
        return level1Time + level2Time;
    }

    public string GetGrade()
    {
        float total = GetTotalTime();
        float penalty = retries * 2f;
        float final = total + penalty;

        if (final < 30 && retries == 0) return "S+ (SUPERB!)";
        if (final < 45 && retries <= 1) return "S (EXCELLENT!)";
        if (final < 60 && retries <= 2) return "A (GREAT!)";
        if (final < 90 && retries <= 3) return "B (GOOD!)";
        if (final < 120) return "C (PASSING)";
        return "D (NEED IMPROVEMENT)";
    }
}