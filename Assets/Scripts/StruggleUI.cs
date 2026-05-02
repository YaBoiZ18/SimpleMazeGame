using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StruggleUI : MonoBehaviour
{
    public GameObject root;
    public TMP_Text promptText;
    public TMP_Text keyText;
    public Slider progressBar;

    private float pulseTimer;

    void Update()
    {
        if (!root.activeSelf) return;

        pulseTimer += Time.deltaTime * 6f;

        float scale = 1f + Mathf.Sin(pulseTimer) * 0.08f;
        keyText.transform.localScale = Vector3.one * scale;

        float alpha = 0.7f + Mathf.Sin(pulseTimer * 2f) * 0.3f;
        promptText.color = new Color(1f, alpha, alpha);
    }

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void SetProgress(float value)
    {
        progressBar.value = value;
    }
}