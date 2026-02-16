using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
   [SerializeField] private PlayerController player; // Reference to the PlayerController to access stamina values.
   [SerializeField] private Image staminaFill; // UI Image component representing the fill of the stamina bar.

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            staminaFill.fillAmount = player.StaminaNormalized; // Update the fill amount based on the player's current stamina (0 to 1).
        }
    }
}
