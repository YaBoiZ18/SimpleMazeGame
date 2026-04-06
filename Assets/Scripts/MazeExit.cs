using UnityEngine;

public class MazeExit : MonoBehaviour
{
    private bool isUnlocked = false; // Tracks whether the exit is unlocked

    public void UnlockExit()
    {
        isUnlocked = true;
        gameObject.SetActive(true); // Make visible
        GetComponent<BeaconPulse>().enabled = true; // Enable beacon pulse
    }

    private void Start()
    {
        // Start disabled
        gameObject.SetActive(false);
        GetComponent<BeaconPulse>().enabled = false; // Disable beacon pulse until unlocked
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.Instance.WinGame();
            FindObjectOfType<MazeTimer>().StopTimer();
            float finalTime = FindObjectOfType<MazeTimer>().GetElapsedTime();
            Debug.Log("Maze completed in: " + finalTime + " seconds");
        }
    }
}