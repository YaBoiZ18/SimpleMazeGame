using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60;

    void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync to allow manual frame rate control.
        Application.targetFrameRate = targetFPS; // Set the target frame rate.
    }
}
