using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Credits Panel")]
    public GameObject creditsPanel;

    void Start()
    {
        Debug.Log("=== MAIN MENU START ===");

        // FIX 1: Destroy duplicate EventSystems caused by DontDestroyOnLoad carrying
        // the old one forward — duplicate EventSystems make all buttons stop working.
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"⚠️ Found {eventSystems.Length} EventSystems! Destroying duplicates — this was why buttons stopped working after returning from PlayAgain.");
            for (int i = 1; i < eventSystems.Length; i++)
                Destroy(eventSystems[i].gameObject);
        }

        // FIX 2: Safely reset the TimeManager — NewGame() only if instance exists
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.NewGame();
            Debug.Log("✅ TimeManager reset for new game");
        }
        else
        {
            Debug.LogWarning("⚠️ TimeManager.Instance is NULL on MainMenu Start — make sure TimeManager is in the first scene or persists via DontDestroyOnLoad.");
        }

        // Hide credits panel on start
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        else
            Debug.LogWarning("⚠️ creditsPanel is not assigned in Inspector.");

        // FIX 3: Verify Level1 is in Build Settings before the player even clicks Play
        // (catches missing scene early rather than silently failing on click)
        if (Application.isEditor)
        {
            bool level1Exists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                if (path.Contains("Level1")) { level1Exists = true; break; }
            }
            if (!level1Exists)
                Debug.LogError("❌ 'Level1' scene is NOT in Build Settings! File → Build Settings → Add Open Scenes.");
            else
                Debug.Log("✅ Level1 found in Build Settings");
        }
    }

    public void PlayGame()
    {
        Debug.Log("▶️ PlayGame clicked — loading Level1...");

        // FIX 4: Guard against missing scene crashing silently
        try
        {
            SceneManager.LoadScene("Level1");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load Level1: {e.Message} — Is it added to Build Settings?");
        }
    }

    public void QuitGame()
    {
        Debug.Log("🚪 QuitGame clicked — quitting application");
        // Application.Quit() works in a built app; does nothing in Editor (expected)
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowCredits()
    {
        Debug.Log("📋 ShowCredits clicked");

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            Debug.Log("✅ Credits panel opened");
        }
        else
        {
            Debug.LogError("❌ creditsPanel is NULL — assign Canvas_DevCredits in the Inspector on this MainMenu GameObject.");
        }
    }

    public void HideCredits()
    {
        Debug.Log("📋 HideCredits clicked");

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
            Debug.Log("✅ Credits panel closed");
        }
        else
        {
            Debug.LogError("❌ creditsPanel is NULL on HideCredits — assign it in the Inspector.");
        }
    }
}