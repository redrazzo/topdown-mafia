using MafiaTopDown.Gameplay.Domain.Campaign;
using MafiaTopDown.Gameplay.Domain.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Mission Definition", fileName = "MissionDefinition")]
    public sealed class MissionDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string missionId = string.Empty;
        [SerializeField] private string title = string.Empty;
        [SerializeField] private string summary = string.Empty;
        [SerializeField] private string startingStageId = string.Empty;
        [SerializeField] private int cashReward;
        [SerializeField] private int reputationDelta;
        [SerializeField] private string[] unlockDistrictIds = new string[0];
        [SerializeField] private string[] unlockActivityIds = new string[0];
        [SerializeField] private MissionStageRecord[] stages = new MissionStageRecord[0];
        [SerializeField] private MissionCheckpointRecord[] checkpoints = new MissionCheckpointRecord[0];
        [SerializeField] private FailureConditionRecord[] failureConditions = new FailureConditionRecord[0];

        public string MissionId => missionId;

        public MissionDefinition ToDefinition()
        {
            var definitionStages = new MissionStage[stages.Length];
            for (var index = 0; index < stages.Length; index += 1)
            {
                definitionStages[index] = stages[index].ToDefinition();
            }

            var definitionCheckpoints = new MissionCheckpoint[checkpoints.Length];
            for (var index = 0; index < checkpoints.Length; index += 1)
            {
                definitionCheckpoints[index] = checkpoints[index].ToDefinition();
            }

            var definitionFailureConditions = new FailureCondition[failureConditions.Length];
            for (var index = 0; index < failureConditions.Length; index += 1)
            {
                definitionFailureConditions[index] = failureConditions[index].ToDefinition();
            }

            return new MissionDefinition(
                missionId,
                title,
                summary,
                startingStageId,
                new RewardPayload(cashReward, reputationDelta, unlockDistrictIds, unlockActivityIds),
                definitionStages,
                definitionCheckpoints,
                definitionFailureConditions);
        }

        [System.Serializable]
        public sealed class MissionStageRecord
        {
            public string Id = string.Empty;
            public string Title = string.Empty;
            public string ObjectiveText = string.Empty;
            public string NextStageId = string.Empty;
            public bool CreatesCheckpoint;

            public MissionStage ToDefinition()
            {
                return new MissionStage(Id, Title, ObjectiveText, NextStageId, CreatesCheckpoint);
            }
        }

        [System.Serializable]
        public sealed class MissionCheckpointRecord
        {
            public string Id = string.Empty;
            public string SceneName = string.Empty;
            public string SpawnPointId = string.Empty;
            public string MissionStageId = string.Empty;

            public MissionCheckpoint ToDefinition()
            {
                return new MissionCheckpoint(Id, SceneName, SpawnPointId, MissionStageId);
            }
        }

        [System.Serializable]
        public sealed class FailureConditionRecord
        {
            public string Id = string.Empty;
            public FailureConditionType Type;
            public string Description = string.Empty;

            public FailureCondition ToDefinition()
            {
                return new FailureCondition(Id, Type, Description);
            }
        }
    }
}
