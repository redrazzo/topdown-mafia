using MafiaTopDown.Gameplay.Runtime.Combat;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Missions
{
    public sealed class EliminateTargetsObjective : MonoBehaviour
    {
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private string requiredObjectiveId = string.Empty;
        [SerializeField] private CombatHealth[] targets = new CombatHealth[0];
        [SerializeField] private CampaignProgressionController campaignProgressionController;
        [SerializeField] private MissionDefinitionAsset missionAsset;
        [SerializeField] private string missionStageIdToComplete = string.Empty;

        private bool _hasCompleted;

        private void Update()
        {
            if (_hasCompleted || objectiveSystem == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(requiredObjectiveId) &&
                objectiveSystem.CurrentObjectiveId != requiredObjectiveId)
            {
                return;
            }

            foreach (var target in targets)
            {
                if (target != null && target.IsAlive)
                {
                    return;
                }
            }

            objectiveSystem.CompleteCurrentObjective();
            if (campaignProgressionController != null && missionAsset != null)
            {
                campaignProgressionController.CompleteMissionStage(missionAsset, missionStageIdToComplete);
            }

            _hasCompleted = true;
        }
    }
}
