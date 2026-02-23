using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Playing,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    [Header("References")]
    [SerializeField] private MazeGenerator mazeGenerator;

    private void Awake()
    {
        // Singleton pattern (one GameManager only)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Start);
        StartGame();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Start:
                Debug.Log("Game Started");
                break;

            case GameState.Playing:
                Debug.Log("Game Playing");
                break;

            case GameState.Win:
                Debug.Log("You Win!");
                break;

            case GameState.Lose:
                Debug.Log("You Lose!");
                break;
        }
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void WinGame()
    {
        if (CurrentState != GameState.Playing) return;

        SetState(GameState.Win);
        Invoke(nameof(RestartGame), 3f);
    }

    public void LoseGame()
    {
        if (CurrentState != GameState.Playing) return;

        SetState(GameState.Lose);
        Invoke(nameof(RestartGame), 3f);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}