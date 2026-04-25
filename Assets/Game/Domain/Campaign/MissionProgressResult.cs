using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class MissionProgressResult
    {
        public MissionProgressResult(bool missionCompleted, RewardPayload completionReward)
        {
            MissionCompleted = missionCompleted;
            CompletionReward = completionReward;
        }

        public bool MissionCompleted { get; }

        public RewardPayload CompletionReward { get; }
    }
}
