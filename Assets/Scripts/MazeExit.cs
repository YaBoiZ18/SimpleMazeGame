using UnityEngine;

public class MazeExit : MonoBehaviour
{
    private bool isUnlocked = false;

    public void UnlockExit()
    {
        isUnlocked = true;
        gameObject.SetActive(true); // Make visible
    }

    private void Start()
    {
        // Start disabled
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.Instance.WinGame();
        }
    }
}