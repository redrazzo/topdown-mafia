namespace MafiaTopDown.Gameplay.Domain.World
{
    public sealed class ActivityDefinition
    {
        public ActivityDefinition(
            string id,
            string displayName,
            ActivityType activityType,
            string districtId,
            int rewardCash,
            bool isRepeatable,
            string unlockMissionId)
        {
            Id = id;
            DisplayName = displayName;
            ActivityType = activityType;
            DistrictId = districtId;
            RewardCash = rewardCash;
            IsRepeatable = isRepeatable;
            UnlockMissionId = unlockMissionId;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public ActivityType ActivityType { get; }

        public string DistrictId { get; }

        public int RewardCash { get; }

        public bool IsRepeatable { get; }

        public string UnlockMissionId { get; }
    }

    public enum ActivityType
    {
        CourierJob = 0,
        DebtCollection = 1,
        GetawayContract = 2,
        ChopDelivery = 3,
        Lookout = 4
    }
}
