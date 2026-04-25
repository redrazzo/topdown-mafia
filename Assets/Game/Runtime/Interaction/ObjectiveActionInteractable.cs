using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class ObjectiveActionInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private string disabledReason = "You are not ready for this yet.";
        [SerializeField] private string logMessage = string.Empty;
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private CampaignProgressionController campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        public bool CanInteract
        {
            get
            {
                if (objectiveSystem == null || string.IsNullOrWhiteSpace(requiredObjectiveId))
                {
                    return true;
                }

                return objectiveSystem.CurrentObjectiveId == requiredObjectiveId;
            }
        }

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

            if (!string.IsNullOrWhiteSpace(logMessage))
            {
                Debug.Log(logMessage);
            }

            if (objectiveSystem != null)
            {
                objectiveSystem.CompleteCurrentObjective();
            }

            if (campaignProgressionController != null && missionAsset != null)
            {
                campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdToComplete);
            }
        }
    }
}
