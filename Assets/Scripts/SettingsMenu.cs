using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public TMP_Text sensitivityText;

    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject howToPlayPanel;

    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        LoadSettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        SaveSettings();
    }

    public void OpenHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);

        UpdateSensitivityLabel(value);

        // LIVE UPDATE PLAYER
        if (player != null)
        {
            player.SetSensitivity(value);
        }
    }

    void UpdateSensitivityLabel(float value)
    {
        string level;

        if (value < 250)
            level = "Low";
        else if (value < 450)
            level = "Normal";
        else if (value < 700)
            level = "High";
        else
            level = "Insane";

        sensitivityText.text =
            "Sensitivity: " + level + " (" + Mathf.RoundToInt(value) + ")";
    }


    // Save settings to PlayerPrefs
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);

        PlayerPrefs.Save();
    }

    // Load settings from PlayerPrefs
    void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 400f);
        int fullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        int quality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());

        volumeSlider.value = volume;
        sensitivitySlider.value = sensitivity;
        UpdateSensitivityLabel(sensitivity);

        AudioListener.volume = volume;
        Screen.fullScreen = fullscreen == 1;
        QualitySettings.SetQualityLevel(quality);
    }
}