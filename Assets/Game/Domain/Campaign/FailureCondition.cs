namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class FailureCondition
    {
        public FailureCondition(string id, FailureConditionType type, string description)
        {
            Id = id;
            Type = type;
            Description = description;
        }

        public string Id { get; }

        public FailureConditionType Type { get; }

        public string Description { get; }
    }

    public enum FailureConditionType
    {
        None = 0,
        VehicleDestroyed = 1,
        PlayerDead = 2,
        TargetEscaped = 3,
        TimerExpired = 4
    }
}
