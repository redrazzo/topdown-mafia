using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public sealed class ActivityHudController : MonoBehaviour
    {
        [SerializeField] private ActivityProgressionController activityProgressionController;
        [SerializeField] private Rect activityRect = new Rect(24f, 210f, 320f, 62f);

        private void OnGUI()
        {
            if (activityProgressionController == null || string.IsNullOrWhiteSpace(activityProgressionController.CurrentActivityDisplayName))
            {
                return;
            }

            HudStyleUtility.DrawPanel(
                activityRect,
                "Side Ledger",
                activityProgressionController.CurrentActivityDisplayName);
        }
    }
}
