using System.Collections.Generic;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class MissionProgressTracker
    {
        private readonly List<string> _completedStageIds = new List<string>();

        public MissionProgressTracker(MissionDefinition missionDefinition)
        {
            MissionDefinition = missionDefinition;
            CurrentStageId = missionDefinition.StartingStageId;
        }

        public MissionDefinition MissionDefinition { get; }

        public string CurrentStageId { get; private set; }

        public bool IsComplete => CurrentStageId == null;

        public IReadOnlyList<string> CompletedStageIds => _completedStageIds;

        public MissionProgressResult CompleteCurrentStage()
        {
            if (CurrentStageId == null)
            {
                return new MissionProgressResult(true, MissionDefinition.CompletionReward);
            }

            var stage = MissionDefinition.GetStage(CurrentStageId);
            _completedStageIds.Add(stage.Id);
            CurrentStageId = string.IsNullOrWhiteSpace(stage.NextStageId) ? null : stage.NextStageId;

            if (CurrentStageId == null)
            {
                return new MissionProgressResult(true, MissionDefinition.CompletionReward);
            }

            return new MissionProgressResult(false, RewardPayload.Empty);
        }
    }
}
