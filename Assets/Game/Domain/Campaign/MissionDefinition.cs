using System;
using System.Collections.Generic;
using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class MissionDefinition
    {
        private readonly Dictionary<string, MissionStage> _stagesById;

        public MissionDefinition(
            string id,
            string title,
            string summary,
            string startingStageId,
            RewardPayload completionReward,
            IEnumerable<MissionStage> stages,
            IEnumerable<MissionCheckpoint> checkpoints,
            IEnumerable<FailureCondition> failureConditions)
        {
            Id = id;
            Title = title;
            Summary = summary;
            StartingStageId = startingStageId;
            CompletionReward = completionReward;
            Stages = stages.ToArray();
            Checkpoints = checkpoints.ToArray();
            FailureConditions = failureConditions.ToArray();
            _stagesById = Stages.ToDictionary(stage => stage.Id);

            if (!_stagesById.ContainsKey(startingStageId))
            {
                throw new ArgumentException("Starting stage must exist in the mission stage list.", nameof(startingStageId));
            }
        }

        public string Id { get; }

        public string Title { get; }

        public string Summary { get; }

        public string StartingStageId { get; }

        public RewardPayload CompletionReward { get; }

        public IReadOnlyList<MissionStage> Stages { get; }

        public IReadOnlyList<MissionCheckpoint> Checkpoints { get; }

        public IReadOnlyList<FailureCondition> FailureConditions { get; }

        public MissionStage GetStage(string stageId)
        {
            return _stagesById[stageId];
        }

        public MissionCheckpoint GetCheckpointForStage(string stageId)
        {
            return Checkpoints.FirstOrDefault(checkpoint => checkpoint.MissionStageId == stageId);
        }
    }
}
