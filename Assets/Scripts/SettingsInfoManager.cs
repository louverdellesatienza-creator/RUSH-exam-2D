using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsInfoManager : MonoBehaviour
{
    public static SettingsInfoManager Instance;
    public GameObject settingsPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
            Debug.Log("Settings panel opened");
        }
    }

    public void ClosePanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
            Debug.Log("Settings panel closed");
        }
    }
}