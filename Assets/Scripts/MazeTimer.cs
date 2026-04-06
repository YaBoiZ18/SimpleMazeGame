using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MazeTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign in inspector
    private float elapsedTime = 0f; // Time in seconds
    private bool running = false;

    public void StartTimer() // Call this when the player starts the maze
    {
        elapsedTime = 0f;
        running = true;
    }

    public void StopTimer() // Call this when the player reaches the exit
    {
        running = false;
    }

    private void Update()
    {
        if (!running) return;

        elapsedTime += Time.deltaTime; // Increment elapsed time by the time since last frame
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime); // Convert elapsed time to TimeSpan for easy formatting
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds); // Format as MM:SS
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}