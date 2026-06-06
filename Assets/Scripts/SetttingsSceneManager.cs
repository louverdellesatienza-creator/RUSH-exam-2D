using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsSceneManager : MonoBehaviour
{
    [Header("Pages (3 panels)")]
    public GameObject[] pages; // 0: Character, 1: Story, 2: Controls

    [Header("Navigation Buttons")]
    public Button[] navButtons; // 3 buttons: Character, Story, Controls
    public Button backButton;
    public Button nextButton;
    public Button doneButton;
    public Button cancelButton;

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
        // Check if pages are assigned
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("Pages array is empty! Assign Page_Character, Page_Story, Page_Controls in Inspector.");
            return;
        }

        Debug.Log($"Found {pages.Length} pages");

        // Load saved preference
        selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");
        UpdateCharacterDisplay();

        SetupButtons();
        ShowPage(0);
    }

    void SetupButtons()
    {
        Debug.Log("=== Setting up Settings Scene ===");

        // Character selection buttons
        if (boyButton != null)
            boyButton.onClick.AddListener(() => SelectCharacter("Boy"));

        if (girlButton != null)
            girlButton.onClick.AddListener(() => SelectCharacter("Girl"));

        // Navigation buttons (top navigation)
        if (navButtons != null && navButtons.Length > 0)
        {
            if (navButtons[0] != null)
                navButtons[0].onClick.AddListener(() => ShowPage(0));
            if (navButtons.Length > 1 && navButtons[1] != null)
                navButtons[1].onClick.AddListener(() => ShowPage(1));
            if (navButtons.Length > 2 && navButtons[2] != null)
                navButtons[2].onClick.AddListener(() => ShowPage(2));
        }

        // Back and Next buttons
        if (backButton != null)
            backButton.onClick.AddListener(PreviousPage);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        // Done and Cancel buttons
        if (doneButton != null)
            doneButton.onClick.AddListener(OnDone);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);

        Debug.Log("=== Settings Scene Setup Complete ===");
    }

    void SelectCharacter(string character)
    {
        selectedCharacter = character;
        UpdateCharacterDisplay();
        Debug.Log($"Selected: {character}");
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
        // Check if pages array is valid
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("Cannot show page: Pages array is empty!");
            return;
        }

        // Check if page index is valid
        if (pageIndex < 0 || pageIndex >= pages.Length)
        {
            Debug.LogError($"Page index {pageIndex} is out of range! Pages length: {pages.Length}");
            return;
        }

        currentPage = pageIndex;
        Debug.Log($"Showing page: {currentPage + 1}/{pages.Length}");

        // Hide all pages
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(false);
        }

        // Show selected page
        if (pages[currentPage] != null)
            pages[currentPage].SetActive(true);
        else
            Debug.LogError($"Page {currentPage} is NULL!");

        // Update navigation button colors
        if (navButtons != null)
        {
            for (int i = 0; i < navButtons.Length && i < pages.Length; i++)
            {
                if (navButtons[i] != null)
                {
                    var colors = navButtons[i].colors;
                    colors.normalColor = (i == currentPage) ? Color.green : Color.white;
                    navButtons[i].colors = colors;
                }
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
        if (pages == null || pages.Length == 0) return;

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
        if (pages == null || pages.Length == 0) return;

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

    void OnDone()
    {
        Debug.Log("=== DONE BUTTON CLICKED ===");

        // Save character selection
        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);
        PlayerPrefs.Save();

        // Update GameManager if it exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.selectedCharacter = selectedCharacter;
            Debug.Log("GameManager updated with character: " + selectedCharacter);
        }

        Debug.Log($"Starting Level 1 with character: {selectedCharacter}");

        // Load Level 1 scene
        SceneManager.LoadScene("Level1-outside school");
    }

    void OnCancel()
    {
        Debug.Log("Cancel clicked! Returning to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
}