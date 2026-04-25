namespace MafiaTopDown.Gameplay.Domain.World
{
    public sealed class LandmarkDefinition
    {
        public LandmarkDefinition(
            string id,
            string districtId,
            string displayName,
            LandmarkType landmarkType)
        {
            Id = id;
            DistrictId = districtId;
            DisplayName = displayName;
            LandmarkType = landmarkType;
        }

        public string Id { get; }

        public string DistrictId { get; }

        public string DisplayName { get; }

        public LandmarkType LandmarkType { get; }
    }

    public enum LandmarkType
    {
        Warehouse = 0,
        SocialClub = 1,
        Bank = 2,
        Garage = 3,
        Church = 4
    }
}
