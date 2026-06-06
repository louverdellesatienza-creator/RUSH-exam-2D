using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Main Menu Started");

        if (TimeManager.Instance != null)
            TimeManager.Instance.NewGame();
    }

    public void PlayGame()
    {
        Debug.Log("Going to Settings Scene...");
        SceneManager.LoadScene("SettingsScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}