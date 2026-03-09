using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [SerializeField] private TextMeshProUGUI objectiveText;

    void Awake()
    {
        Instance = this;
    }

    public void SetObjective(string text)
    {
        objectiveText.text = text;
    }
}