using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Collectibles")]
    public int totalCollectibles = 3;
    private int collectedCount = 0;
    private List<Collectible.ItemType> collectedItems = new List<Collectible.ItemType>();

    [Header("UI")]
    public Text collectibleText;
    public GameObject star1, star2, star3;
    public GameObject levelCompletePanel;
    public Text resultText;
    public Text starRatingText;
    public GameObject continueButton;
    public GameObject replayButton;

    [Header("Door")]
    public Door door;
    public int minRequiredToPass = 1;

    private bool levelCompleted = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        levelCompletePanel.SetActive(false);
    }

    public void CollectItem(Collectible.ItemType item)
    {
        if (levelCompleted) return;

        collectedCount++;
        collectedItems.Add(item);
        UpdateUI();

        // Check if all items collected
        if (collectedCount >= totalCollectibles)
        {
            door.UnlockDoor();
        }
    }

    void UpdateUI()
    {
        if (collectibleText != null)
            collectibleText.text = $"Items: {collectedCount}/{totalCollectibles}";

        // Update star display based on collected count
        if (star1 != null) star1.SetActive(collectedCount >= 1);
        if (star2 != null) star2.SetActive(collectedCount >= 2);
        if (star3 != null) star3.SetActive(collectedCount >= 3);
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        Time.timeScale = 0f; // Pause game

        levelCompletePanel.SetActive(true);

        // Set star rating text
        string rating = "";
        string grade = "";

        switch (collectedCount)
        {
            case 3:
                rating = "⭐⭐⭐ PERFECT!";
                grade = "A+";
                break;
            case 2:
                rating = "⭐⭐ GOOD!";
                grade = "B";
                break;
            case 1:
                rating = "⭐ PASSING";
                grade = "C";
                break;
            default:
                rating = "⭐ NO STARS";
                grade = "F";
                break;
        }

        starRatingText.text = rating;

        if (collectedCount >= minRequiredToPass)
        {
            resultText.text = $"Items Collected: {collectedCount}/{totalCollectibles}\nGrade: {grade}";
            continueButton.SetActive(true);
            replayButton.SetActive(true);
        }
        else
        {
            resultText.text = $"FAILED!\nItems: {collectedCount}/{totalCollectibles}\nNeed at least {minRequiredToPass} item!";
            continueButton.SetActive(false);
            replayButton.SetActive(true);
        }
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;
        // Save star rating to GameManager
        GameManager.Instance.SetLevel1Stars(collectedCount);
        SceneManager.LoadScene("Level2");
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool CanCompleteLevel()
    {
        return collectedCount >= minRequiredToPass;
    }
}