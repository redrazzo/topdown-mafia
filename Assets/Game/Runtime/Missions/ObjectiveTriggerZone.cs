using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Missions
{
    [DisallowMultipleComponent]
    public sealed class ObjectiveTriggerZone : MonoBehaviour
    {
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private CampaignProgressionController campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        private bool _hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered && triggerOnce)
            {
                return;
            }

            if (other.GetComponentInParent<TopDownPlayerController>() == null)
            {
                return;
            }

            if (objectiveSystem == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(requiredObjectiveId) &&
                objectiveSystem.CurrentObjectiveId != requiredObjectiveId)
            {
                return;
            }

            objectiveSystem.CompleteCurrentObjective();
            if (campaignProgressionController != null && missionAsset != null)
            {
                campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdToComplete);
            }

            _hasTriggered = true;
        }
    }
}
