using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string selectedCharacter = "Boy";
    public int level1Stars = 0;
    public int level2Stars = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLevel1Stars(int collected)
    {
        level1Stars = collected;
        Debug.Log($"Level 1 completed with {level1Stars} stars");
    }

    public void SetLevel2Stars(int collected)
    {
        level2Stars = collected;
        Debug.Log($"Level 2 completed with {level2Stars} stars");
    }
}