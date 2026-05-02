using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject winScreen;

    [SerializeField] private GameObject gameOverScreen;

    // Struggle mechanic variables
    [SerializeField] private StruggleUI struggleUI;
    private bool struggling = false;
    private int mashCount = 0;
    private int requiredMash = 8;

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

    // Struggle mechanic implementation
    public void StartStruggle(PlayerController player, EnemyAI enemy)
    {
        if (struggling) return;

        StartCoroutine(StruggleRoutine(player, enemy));
    }

    // Coroutine to handle the struggle mechanic
    IEnumerator StruggleRoutine(PlayerController player, EnemyAI enemy)
    {
        struggling = true;

        player.enabled = false;
        enemy.enabled = false;

        struggleUI.Show();

        mashCount = 0;

        float timer = 1.5f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E))
            {
                mashCount++;
                struggleUI.SetProgress((float)mashCount / requiredMash);

                if (mashCount >= requiredMash)
                {
                    EscapePlayer(player, enemy);
                    yield break;
                }
            }

            yield return null;
        }

        LoseGame();
    }

    // Handle successful escape from the enemy
    void EscapePlayer(PlayerController player, EnemyAI enemy)
    {
        struggleUI.Hide();

        player.enabled = true;

        enemy.IncreaseSpeed();
        enemy.ResetCatch();

        StartCoroutine(StunEnemy(enemy));

        struggling = false;
    }

    // Stun the enemy for a short duration after the player escapes
    IEnumerator StunEnemy(EnemyAI enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        agent.isStopped = true;

        yield return new WaitForSeconds(2f);

        agent.isStopped = false;
        enemy.enabled = true;
    }

    // Handle losing the game (e.g. player captured)
    public void LoseGame()
    {
        gameOverScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}