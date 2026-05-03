using MafiaTopDown.Gameplay.Runtime.Combat;
using MafiaTopDown.Gameplay.Runtime.Law;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public sealed class StatusHudController : MonoBehaviour
    {
        [SerializeField] private CombatHealth playerHealth;
        [SerializeField] private HeatSystemController heatSystemController;
        [SerializeField] private PoliceResponseController policeResponseController;
        [SerializeField] private Rect statusRect = new Rect(20f, 98f, 230f, 88f);

        private void OnGUI()
        {
            var healthText = playerHealth == null
                ? "n/a"
                : playerHealth.CurrentHealth + " / " + playerHealth.MaximumHealth;
            var heatText = heatSystemController == null
                ? "None"
                : heatSystemController.CurrentTier + " (" + heatSystemController.CurrentIntensity + ")";
            var policeText = policeResponseController == null
                ? "Clear"
                : policeResponseController.CurrentStatusText;

            HudStyleUtility.DrawPanel(
                statusRect,
                "Street Read",
                "Health  " + healthText + "\nHeat    " + heatText + "\nLaw     " + policeText);
        }
    }
}
