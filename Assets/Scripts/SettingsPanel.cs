using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsPanelManager : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject settingsPanel;

    [Header("Pages (3 panels)")]
    public GameObject[] pages; // 0: Character, 1: Story, 2: Controls

    [Header("Navigation Buttons")]
    public Button[] navButtons; // 3 buttons: Character, Story, Controls
    public Button backButton;
    public Button nextButton;
    public Button closeButton;

    [Header("Character Selection")]
    public Button boyButton;
    public Button girlButton;
    public Image characterPreview;
    public Sprite boySprite;
    public Sprite girlSprite;
    public TextMeshProUGUI selectionStatus;

    private int currentPage = 0;
    private string selectedCharacter = "Boy";

    void Start()
    {
        LoadPreferences();
        SetupButtons();
        ShowPage(0);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void LoadPreferences()
    {
        selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");
        UpdateCharacterDisplay();
        Debug.Log($"Loaded character: {selectedCharacter}");
    }

    void SavePreferences()
    {
        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);
        PlayerPrefs.Save();
        Debug.Log($"Saved character: {selectedCharacter}");
    }

    void SetupButtons()
    {
        Debug.Log("=== Setting up Settings Panel Manager ===");

        // Character selection buttons
        if (boyButton != null)
        {
            boyButton.onClick.RemoveAllListeners();
            boyButton.onClick.AddListener(() => SelectCharacter("Boy"));
            Debug.Log("BoyButton configured");
        }

        if (girlButton != null)
        {
            girlButton.onClick.RemoveAllListeners();
            girlButton.onClick.AddListener(() => SelectCharacter("Girl"));
            Debug.Log("GirlButton configured");
        }

        // Navigation buttons (top navigation)
        if (navButtons.Length > 0 && navButtons[0] != null)
            navButtons[0].onClick.AddListener(() => ShowPage(0));
        if (navButtons.Length > 1 && navButtons[1] != null)
            navButtons[1].onClick.AddListener(() => ShowPage(1));
        if (navButtons.Length > 2 && navButtons[2] != null)
            navButtons[2].onClick.AddListener(() => ShowPage(2));

        // Back and Next buttons
        if (backButton != null)
            backButton.onClick.AddListener(PreviousPage);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        // Close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
            Debug.Log("CloseButton configured");
        }

        Debug.Log("=== Settings Panel Setup Complete ===");
    }

    void SelectCharacter(string character)
    {
        Debug.Log($"Character selected: {character}");
        selectedCharacter = character;
        UpdateCharacterDisplay();
        SavePreferences();

        if (GameManager.Instance != null)
            GameManager.Instance.selectedCharacter = character;
    }

    void UpdateCharacterDisplay()
    {
        // Update preview image
        if (characterPreview != null)
        {
            characterPreview.sprite = selectedCharacter == "Boy" ? boySprite : girlSprite;
        }

        // Update status text
        if (selectionStatus != null)
        {
            selectionStatus.text = $"Selected: {selectedCharacter}";
        }

        // Update button colors
        if (boyButton != null)
        {
            var colors = boyButton.colors;
            colors.normalColor = selectedCharacter == "Boy" ? Color.green : Color.white;
            boyButton.colors = colors;
        }

        if (girlButton != null)
        {
            var colors = girlButton.colors;
            colors.normalColor = selectedCharacter == "Girl" ? Color.green : Color.white;
            girlButton.colors = colors;
        }
    }

    void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;
        Debug.Log($"Showing page: {pageIndex + 1}/3");

        // Hide all pages
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(false);
        }

        // Show selected page
        if (pages[currentPage] != null)
            pages[currentPage].SetActive(true);

        // Update navigation button colors
        for (int i = 0; i < navButtons.Length; i++)
        {
            if (navButtons[i] != null)
            {
                var colors = navButtons[i].colors;
                colors.normalColor = (i == currentPage) ? Color.green : Color.white;
                navButtons[i].colors = colors;
            }
        }

        // Update Back/Next button states
        if (backButton != null)
            backButton.interactable = (currentPage > 0);

        if (nextButton != null)
            nextButton.interactable = (currentPage < pages.Length - 1);
    }

    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
        else
        {
            // Wrap to last page
            currentPage = pages.Length - 1;
            ShowPage(currentPage);
        }
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
        else
        {
            // Wrap to first page
            currentPage = 0;
            ShowPage(currentPage);
        }
    }

    public void ShowPanel()
    {
        Debug.Log("ShowPanel called");

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            ShowPage(0);
            Debug.Log("Settings panel opened");
        }
        else
        {
            Debug.LogError("settingsPanel is NULL!");
        }
    }

    public void ClosePanel()
    {
        Debug.Log("ClosePanel called");

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            SavePreferences();

            // Notify LevelManager to close settings and resume game
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.CloseSettings();
            }

            Debug.Log("Settings panel closed");
        }
        else
        {
            Debug.LogError("settingsPanel is NULL!");
        }
    }
}