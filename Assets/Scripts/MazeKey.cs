using UnityEngine;

public class MazeKey : MonoBehaviour
{
    // Reference to the maze exit that this key will unlock.
    private MazeExit exit;

    // Called by the spawner or initializer to set which exit this key unlocks.
    public void Initialize(MazeExit targetExit)
    {
        exit = targetExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a PlayerController component (is the player).
        if (other.GetComponent<PlayerController>() != null)
        {
            // If we have a valid exit reference, unlock it.
            if (exit != null)
                exit.UnlockExit();

            // Remove the key object from the scene so it can't be reused.
            Destroy(gameObject);

            // Inform the player via the objective UI that the exit is unlocked.
            ObjectiveUI.Instance.SetObjective("The exit is unlocked! Escape the maze!");
        }
    }
}