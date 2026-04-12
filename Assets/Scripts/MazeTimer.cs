using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MazeTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Reference to the UI TextMeshPro component for displaying the timer

    private float elapsedTime = 0f; // Tracks the elapsed time in seconds
    private bool running = false; // Indicates whether the timer is currently running

    private const string BEST_TIME_KEY = "BestTime"; // Key for storing best time in PlayerPrefs

    public void StartTimer()
    {
        elapsedTime = 0f;
        running = true;
    }

    public void StopTimer()
    {
        running = false;

        float bestTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, float.MaxValue); // Get the current best time, defaulting to a very high value if not set

        if (elapsedTime < bestTime) // If the current elapsed time is better than the best time, update it
        {
            PlayerPrefs.SetFloat(BEST_TIME_KEY, elapsedTime);
            PlayerPrefs.Save();

            Debug.Log("New Best Time!");
        }
    }

    private void Update()
    {
        if (!running) return;

        elapsedTime += Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds); // Format the time as MM:SS
    }

    public float GetElapsedTime() // Method to retrieve the current elapsed time
    {
        return elapsedTime;
    }

    public float GetBestTime() // Method to retrieve the best time from PlayerPrefs
    {
        return PlayerPrefs.GetFloat(BEST_TIME_KEY, float.MaxValue);
    }
}