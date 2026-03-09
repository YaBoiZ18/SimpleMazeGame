using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    // Singleton instance for easy global access to the objective UI.
    public static ObjectiveUI Instance;

    // Reference to the TMP UI text component that displays the objective.
    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Awake()
    {
        // Set the singleton instance to this object.
        Instance = this;
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
        {
            // Assign the provided text to the UI element.
            objectiveText.text = text;
        }
        else
        {
            // Warn if the TMP reference hasn't been set in the inspector.
            Debug.LogWarning("ObjectiveUI: 'objectiveText' is not assigned in the inspector.");
        }
    }
}