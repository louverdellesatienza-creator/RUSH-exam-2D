using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1-outside school");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!"); // Only shows in editor
    }
}