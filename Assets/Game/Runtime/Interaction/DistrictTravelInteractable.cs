using System.Linq;
using MafiaTopDown.Gameplay.Domain.Interactions;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Interaction
{
    public sealed class DistrictTravelInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Travel";
        [SerializeField] private string actionName = "Interact";
        [SerializeField] private string requiredDistrictId = string.Empty;
        [SerializeField] private string lockedReason = "District is not available yet.";
        [SerializeField] private string targetScene = string.Empty;
        [SerializeField] private string spawnPointId = "DefaultSpawn";
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private SaveGameFileService? saveGameFileService;
        [SerializeField] private SceneTransitionController? sceneTransitionController;
        [SerializeField] private SimpleObjectiveSystem? objectiveSystem;
        [SerializeField] private CampaignProgressionController? campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset? missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        public bool CanInteract
        {
            get
            {
                if (sceneTransitionController == null || saveGameFileService == null || string.IsNullOrWhiteSpace(targetScene))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(requiredDistrictId))
                {
                    return true;
                }

                var save = saveGameFileService.LoadOrCreateSave();
                if (!save.UnlockedDistrictIds.Contains(requiredDistrictId))
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

            return InteractionPromptData.Disabled(promptText, actionName, lockedReason);
        }

        public void Interact()
        {
            if (!CanInteract || sceneTransitionController == null)
            {
                return;
            }

            var activeSceneName = SceneManager.GetActiveScene().name;
            var resolvedTargetScene = targetScene;
            var resolvedSpawnPointId = spawnPointId;

            if (campaignProgressionController != null && missionAsset != null)
            {
                var save = campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdToComplete);
                if (save != null &&
                    save.CurrentMissionId != missionAsset.MissionId &&
                    !string.IsNullOrWhiteSpace(save.LastSceneName) &&
                    save.LastSceneName != activeSceneName)
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

            sceneTransitionController.TransitionToExteriorInteriorPair(
                activeSceneName,
                resolvedTargetScene,
                resolvedSpawnPointId);
        }
    }
}
