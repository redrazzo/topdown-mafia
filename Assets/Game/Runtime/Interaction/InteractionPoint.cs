using MafiaTopDown.Gameplay.Domain.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class InteractionPoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private bool canInteract = true;
        [SerializeField] private string disabledReason = string.Empty;
        [SerializeField] private UnityEvent onInteract = new();

        public bool CanInteract => canInteract;

        public InteractionPromptData GetPrompt()
        {
            if (canInteract)
            {
                return InteractionPromptData.Enabled(promptText, actionName);
            }

            var reason = string.IsNullOrWhiteSpace(disabledReason)
                ? "This interaction is currently unavailable."
                : disabledReason;

            return InteractionPromptData.Disabled(promptText, actionName, reason);
        }

        public void Interact()
        {
            if (!canInteract)
            {
                return;
            }

            onInteract.Invoke();
        }

        public void SetInteractable(bool enabled, string reason = "")
        {
            canInteract = enabled;
            disabledReason = reason;
        }
    }
}
