using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.NewGame();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1-outside school");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}