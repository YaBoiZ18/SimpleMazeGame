using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    // Assign this in the Inspector: Image whose fillAmount represents stamina (0..1).
    [SerializeField] private Image staminaFill;

    // Runtime reference to the player whose stamina we display.
    private PlayerController player;

    // Called by external code to bind a PlayerController to this UI.
    public void SetPlayer(PlayerController newPlayer)
    {
        player = newPlayer;
    }

    // Update is called once per frame by Unity.
    // We update the UI image fill to match the player's normalized stamina.
    private void Update()
    {
        // Safety checks to avoid null reference exceptions in edit/play mode.
        if (player == null || staminaFill == null)
        {
            return;
        }

        // Pull the normalized stamina value (expected in range [0,1])
        // and apply it to the UI image's fill amount.
        staminaFill.fillAmount = player.StaminaNormalized;
    }
}