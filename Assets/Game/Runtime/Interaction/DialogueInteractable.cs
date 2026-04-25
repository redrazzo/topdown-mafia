using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Story;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Talk";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private string disabledReason = "Not now.";
        [SerializeField] private bool completeOnlyOnce = true;
        [SerializeField] private DialogueController? dialogueController;
        [SerializeField] private DialogueSequenceAsset? dialogueSequence;
        [SerializeField] private SimpleObjectiveSystem? objectiveSystem;
        [SerializeField] private CampaignProgressionController? campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset? missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        private bool _hasCompleted;

        public bool CanInteract
        {
            get
            {
                if (dialogueController == null || dialogueSequence == null || dialogueController.IsActive)
                {
                    return false;
                }

                if (completeOnlyOnce && _hasCompleted)
                {
                    return false;
                }

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

            if (completeOnlyOnce && _hasCompleted)
            {
                return InteractionPromptData.Disabled(promptText, actionName, "Already handled.");
            }

            return InteractionPromptData.Disabled(promptText, actionName, disabledReason);
        }

        public void Interact()
        {
            if (!CanInteract || dialogueController == null || dialogueSequence == null)
            {
                return;
            }

            dialogueController.StartSequence(dialogueSequence, OnDialogueComplete);
        }

        private void OnDialogueComplete()
        {
            _hasCompleted = true;

            if (objectiveSystem != null &&
                (string.IsNullOrWhiteSpace(requiredObjectiveId) || objectiveSystem.CurrentObjectiveId == requiredObjectiveId))
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
