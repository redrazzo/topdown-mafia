using System.Collections.Generic;
using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.World
{
    public static class WorldUnlockService
    {
        public static SaveGameData ApplyMissionUnlocks(
            SaveGameData saveGameData,
            IEnumerable<DistrictDefinition> districts,
            IEnumerable<ActivityDefinition> activities)
        {
            var unlockedDistrictIds = districts
                .Where(district => district.UnlockedByDefault || saveGameData.CompletedMissionIds.Contains(district.UnlockMissionId))
                .Select(district => district.Id);

            var unlockedActivityIds = activities
                .Where(activity =>
                    string.IsNullOrWhiteSpace(activity.UnlockMissionId) ||
                    saveGameData.CompletedMissionIds.Contains(activity.UnlockMissionId))
                .Select(activity => activity.Id);

            return new SaveGameData(
                currentMissionId: saveGameData.CurrentMissionId,
                currentMissionStageId: saveGameData.CurrentMissionStageId,
                lastSceneName: saveGameData.LastSceneName,
                lastSpawnPointId: saveGameData.LastSpawnPointId,
                cash: saveGameData.Cash,
                reputation: saveGameData.Reputation,
                heatLevel: saveGameData.HeatLevel,
                completedMissionIds: saveGameData.CompletedMissionIds,
                unlockedDistrictIds: MergeDistinct(saveGameData.UnlockedDistrictIds, unlockedDistrictIds),
                unlockedActivityIds: MergeDistinct(saveGameData.UnlockedActivityIds, unlockedActivityIds),
                currentChapterId: saveGameData.CurrentChapterId,
                completedChapterIds: saveGameData.CompletedChapterIds,
                currentActivityId: saveGameData.CurrentActivityId,
                completedActivityIds: saveGameData.CompletedActivityIds);
        }

        private static IReadOnlyList<string> MergeDistinct(IEnumerable<string> left, IEnumerable<string> right)
        {
            return left
                .Concat(right)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .ToArray();
        }
    }
}
