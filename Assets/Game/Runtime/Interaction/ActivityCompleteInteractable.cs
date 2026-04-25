using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class ActivityCompleteInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Complete side job";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private string disabledReason = "You are not carrying the right job.";
        [SerializeField] private ActivityDefinitionAsset activityAsset;
        [SerializeField] private ActivityProgressionController activityProgressionController;

        public bool CanInteract => activityProgressionController != null && activityAsset != null && activityProgressionController.CanComplete(activityAsset);

        public InteractionPromptData GetPrompt()
        {
            if (CanInteract)
            {
                return InteractionPromptData.Enabled(promptText, actionName);
            }

            return InteractionPromptData.Disabled(promptText, actionName, disabledReason);
        }

        public void Interact()
        {
            if (!CanInteract)
            {
                return;
            }

            activityProgressionController.CompleteActivity(activityAsset);
        }
    }
}
