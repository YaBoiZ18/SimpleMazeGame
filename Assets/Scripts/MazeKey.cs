using UnityEngine;

public class MazeKey : MonoBehaviour
{
    // Reference to the maze exit that this key will unlock.
    private MazeExit exit;

    // Reference to the enemy prefab to spawn when the key is collected.
    public GameObject enemyPrefab;

    // Reference to the maze generator to spawn enemies in the maze when the key is collected.
    private MazeGenerator generator;

    // Called by the spawner or initializer to set which exit this key unlocks.
    public void Initialize(MazeExit targetExit, MazeGenerator gen, GameObject enemy)
    {
        exit = targetExit;
        generator = gen;
        enemyPrefab = enemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a PlayerController component (is the player).
        if (other.GetComponent<PlayerController>() != null)
        {
            // If we have a valid exit reference, unlock it.
            if (exit != null)
                exit.UnlockExit();

            // Spawn enemy
            if (generator != null && enemyPrefab != null)
            {
                generator.SpawnEnemy(enemyPrefab);
                generator.TriggerDangerMode();
            }
               

            // Remove the key object from the scene so it can't be reused.
            Destroy(gameObject);

            // Inform the player via the objective UI that the exit is unlocked.
            ObjectiveUI.Instance.SetObjective("The exit is unlocked! Escape the maze!");
        }
    }
}