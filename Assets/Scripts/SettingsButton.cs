using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    void Start()
    {
        // Get the button component
        Button button = GetComponent<Button>();

        // Add click listener
        if (button != null)
        {
            button.onClick.AddListener(OpenSettings);
        }
    }

    void OpenSettings()
    {
        Debug.Log("Settings button clicked!");

        // Find and open the settings panel
        if (SettingsInfoManager.Instance != null)
        {
            SettingsInfoManager.Instance.ShowPanel();
        }
        else
        {
            Debug.LogWarning("SettingsInfoManager not found in scene!");
        }
    }
}