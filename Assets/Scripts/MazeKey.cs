using UnityEngine;

public class MazeKey : MonoBehaviour
{
    private MazeExit exit;

    public void Initialize(MazeExit targetExit)
    {
        exit = targetExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (exit != null)
                exit.UnlockExit();

            Destroy(gameObject); // Remove key
        }
    }
}