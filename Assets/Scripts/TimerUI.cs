using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Debug.Log("TimerUI attached to: " + gameObject.name);
    }

    void Update()
    {
        if (TimeManager.Instance != null)
        {
            float time = TimeManager.Instance.currentLevelTime;  // ← Changed from currentTime to currentLevelTime
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milli = Mathf.FloorToInt((time * 100) % 100);
            textComponent.text = $"TIME: {minutes:00}:{seconds:00}:{milli:00}";
        }
    }
}