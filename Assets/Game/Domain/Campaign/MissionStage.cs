namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class MissionStage
    {
        public MissionStage(
            string id,
            string title,
            string objectiveText,
            string nextStageId,
            bool createsCheckpoint)
        {
            Id = id;
            Title = title;
            ObjectiveText = objectiveText;
            NextStageId = nextStageId;
            CreatesCheckpoint = createsCheckpoint;
        }

        public string Id { get; }

        public string Title { get; }

        public string ObjectiveText { get; }

        public string NextStageId { get; }

        public bool CreatesCheckpoint { get; }
    }
}
