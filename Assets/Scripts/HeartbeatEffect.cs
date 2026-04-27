using UnityEngine;
using UnityEngine.UI;

public class HeartbeatEffect : MonoBehaviour
{
    // UI Image used as the vignette overlay
    public Image vignette;

    // References to transforms for distance checks
    public Transform player;
    public Transform enemy;

    // Maximum distance at which the heartbeat effect is active
    public float maxRange = 14f;

    // Pulse speed when the enemy is far vs near
    public float pulseSpeedFar = 1.2f;
    public float pulseSpeedNear = 3.5f;

    // Maximum alpha for the vignette when at closest proximity
    public float maxAlpha = 0.45f;

    void Start()
    {
        // Ensure vignette starts fully transparent
        SetAlpha(0f);
        // No enemy tracked initially
        enemy = null;
    }

    void Update()
    {
        // If any required reference is missing, disable the effect
        if (enemy == null || player == null || vignette == null)
        {
            SetAlpha(0f);
            return;
        }

        // Calculate distance between player and enemy
        float dist = Vector3.Distance(player.position, enemy.position);

        // If out of range, disable effect
        if (dist > maxRange)
        {
            SetAlpha(0f);
            return;
        }

        // Convert distance to proximity (0 = far at maxRange, 1 = overlapping)
        float proximity = 1f - (dist / maxRange);

        // Interpolate pulse speed based on proximity
        float speed = Mathf.Lerp(pulseSpeedFar, pulseSpeedNear, proximity);

        // Smooth pulse using a sine wave (value between 0 and 1)
        float pulse = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;

        // Minimum alpha scaled by proximity (subtle base glow)
        float minAlpha = 0.05f * proximity;
        // Maximum pulse amplitude interpolated toward configured maxAlpha
        float maxPulse = Mathf.Lerp(0.12f, maxAlpha, proximity);

        // Final alpha is lerped between min and maxPulse by the smooth pulse
        float alpha = Mathf.Lerp(minAlpha, maxPulse, pulse);

        // Apply computed alpha to vignette
        SetAlpha(alpha);
    }

    void SetAlpha(float a)
    {
        // Update only the alpha channel of the vignette color
        Color c = vignette.color;
        c.a = a;
        vignette.color = c;
    }

    public void SetEnemy(Transform newEnemy)
    {
        // Start tracking a new enemy
        enemy = newEnemy;
    }

    public void ClearEnemy()
    {
        // Stop tracking and clear effect immediately
        enemy = null;
        SetAlpha(0f);
    }
}