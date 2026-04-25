using System.Collections.Generic;
using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.World
{
    public static class WorldTravelPlanner
    {
        public static IReadOnlyList<DistrictDefinition> GetTravelDestinations(
            SaveGameData saveGameData,
            IEnumerable<DistrictDefinition> districts,
            string currentDistrictId)
        {
            return districts
                .Where(district => district.Id != currentDistrictId)
                .Where(district =>
                    district.UnlockedByDefault ||
                    saveGameData.UnlockedDistrictIds.Contains(district.Id))
                .ToArray();
        }
    }
}
