using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsButton : MonoBehaviour, IPointerClickHandler
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        // Ensure Raycast Target is enabled on the Image
        Image img = GetComponent<Image>();
        if (img != null && !img.raycastTarget)
        {
            img.raycastTarget = true;
            Debug.Log("Enabled Raycast Target on Settings Button");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Settings button clicked!");

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