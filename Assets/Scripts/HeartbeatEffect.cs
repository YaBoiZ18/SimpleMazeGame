using UnityEngine;
using UnityEngine.UI;

public class HeartbeatEffect : MonoBehaviour
{
    public Image overlay;

    public Transform player;
    public Transform enemy;

    public float maxRange = 12f;
    public float pulseSpeedFar = 2f;
    public float pulseSpeedNear = 6f;
    public float maxAlpha = 0.35f;

    void Start()
    {
        // Start fully invisible
        SetAlpha(0f);
        enemy = null;
    }

    void Update()
    {
        // No enemy = no heartbeat
        if (enemy == null || player == null || overlay == null)
        {
            SetAlpha(0f);
            return;
        }

        float dist = Vector3.Distance(player.position, enemy.position);

        if (dist > maxRange)
        {
            SetAlpha(0f);
            return;
        }

        float proximity = 1f - (dist / maxRange);

        float speed = Mathf.Lerp(pulseSpeedFar, pulseSpeedNear, proximity);

        float pulse =
            (Mathf.Sin(Time.time * speed * Mathf.PI * 2f) + 1f) * 0.5f;

        float alpha = pulse * Mathf.Lerp(0.05f, maxAlpha, proximity);

        SetAlpha(alpha);
    }

    void SetAlpha(float a)
    {
        Color c = overlay.color;
        c.a = a;
        overlay.color = c;
    }

    public void SetEnemy(Transform newEnemy)
    {
        enemy = newEnemy;
    }

    public void ClearEnemy()
    {
        enemy = null;
        SetAlpha(0f);
    }
}