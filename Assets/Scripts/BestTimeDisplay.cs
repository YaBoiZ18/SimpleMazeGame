using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BestTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText; // Reference to the UI TextMeshPro component for displaying the best time

    private void Start()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue); // Get the best time from PlayerPrefs, defaulting to a very high value if not set

        if (bestTime == float.MaxValue) // If there is no best time recorded, display a placeholder
        {
            bestTimeText.text = "Best Time: --:--";
        }
        else // If there is a best time recorded, format and display it
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(bestTime);
            bestTimeText.text = string.Format(
                "Best Time: {0:D2}:{1:D2}",
                timeSpan.Minutes,
                timeSpan.Seconds
            );
        }
    }
}