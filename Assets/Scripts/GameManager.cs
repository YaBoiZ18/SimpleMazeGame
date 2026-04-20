using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject winScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // OPTIONAL
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() // Ensure time is running when the game starts or restarts
    {
        Time.timeScale = 1f;
    }

    public void WinGame()
    {
        // Show win UI
        winScreen.SetActive(true);

        // Freeze time
        Time.timeScale = 0f;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}