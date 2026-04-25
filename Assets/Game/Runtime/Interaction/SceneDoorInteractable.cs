using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Scenes;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class SceneDoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Enter";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private string sourceScene = "District_01";
        [SerializeField] private string targetScene = "Interior_BackOffice_01";
        [SerializeField] private string spawnPointId = "DefaultSpawn";
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private SceneTransitionController? sceneTransitionController;
        [SerializeField] private SimpleObjectiveSystem? objectiveSystem;
        [SerializeField] private CampaignProgressionController? campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset? missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        public bool CanInteract
        {
            get
            {
                if (sceneTransitionController == null)
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

            return InteractionPromptData.Disabled(promptText, actionName, "Door is not configured.");
        }

        public void Interact()
        {
            if (sceneTransitionController == null)
            {
                return;
            }

            var resolvedTargetScene = targetScene;
            var resolvedSpawnPointId = spawnPointId;

            if (campaignProgressionController != null && missionAsset != null)
            {
                var save = campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdToComplete);
                if (save != null &&
                    save.CurrentMissionId != missionAsset.MissionId &&
                    !string.IsNullOrWhiteSpace(save.LastSceneName) &&
                    save.LastSceneName != sourceScene)
                {
                    resolvedTargetScene = save.LastSceneName;
                    resolvedSpawnPointId = save.LastSpawnPointId;
                }
            }

            if (objectiveSystem != null &&
                (string.IsNullOrWhiteSpace(requiredObjectiveId) || objectiveSystem.CurrentObjectiveId == requiredObjectiveId))
            {
                objectiveSystem.CompleteCurrentObjective();
            }

            sceneTransitionController.TransitionToExteriorInteriorPair(sourceScene, resolvedTargetScene, resolvedSpawnPointId);
        }
    }
}
