using System.Collections.Generic;

namespace MafiaTopDown.Gameplay.Domain.World
{
    public sealed class DistrictDefinition
    {
        public DistrictDefinition(
            string id,
            string displayName,
            string sceneName,
            DistrictTheme theme,
            bool unlockedByDefault,
            string unlockMissionId,
            IEnumerable<string> landmarkIds,
            IEnumerable<string> activityIds)
        {
            Id = id;
            DisplayName = displayName;
            SceneName = sceneName;
            Theme = theme;
            UnlockedByDefault = unlockedByDefault;
            UnlockMissionId = unlockMissionId;
            LandmarkIds = new List<string>(landmarkIds);
            ActivityIds = new List<string>(activityIds);
        }

        public string Id { get; }

        public string DisplayName { get; }

        public string SceneName { get; }

        public DistrictTheme Theme { get; }

        public bool UnlockedByDefault { get; }

        public string UnlockMissionId { get; }

        public IReadOnlyList<string> LandmarkIds { get; }

        public IReadOnlyList<string> ActivityIds { get; }
    }

    public enum DistrictTheme
    {
        Docks = 0,
        BusinessCore = 1,
        Residential = 2,
        RailYard = 3
    }
}
