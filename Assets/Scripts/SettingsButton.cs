using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OpenSettings);
        }
    }

    void OpenSettings()
    {
        Debug.Log("Settings button clicked!");

        // Call LevelManager to open settings
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OpenSettings();
        }
        else
        {
            Debug.LogError("LevelManager.Instance is NULL!");
        }
    }
}