using UnityEngine;
using TMPro;

public class RetryUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (TimeManager.Instance != null)
        {
            textComponent.text = $"RETRIES: {TimeManager.Instance.retryCount}";  // ← Changed from retries to retryCount
        }
    }
}