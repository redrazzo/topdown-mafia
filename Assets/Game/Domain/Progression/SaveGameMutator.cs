using System.Collections.Generic;
using System.Linq;

namespace MafiaTopDown.Gameplay.Domain.Progression
{
    public static class SaveGameMutator
    {
        public static SaveGameData ApplyReward(SaveGameData saveGameData, RewardPayload reward)
        {
            return Copy(
                saveGameData,
                cash: saveGameData.Cash + reward.CashReward,
                reputation: saveGameData.Reputation + reward.ReputationDelta,
                unlockedDistrictIds: MergeDistinct(saveGameData.UnlockedDistrictIds, reward.UnlockDistrictIds),
                unlockedActivityIds: MergeDistinct(saveGameData.UnlockedActivityIds, reward.UnlockActivityIds));
        }

        public static SaveGameData MarkMissionCompleted(SaveGameData saveGameData, string missionId)
        {
            return Copy(
                saveGameData,
                completedMissionIds: MergeDistinct(saveGameData.CompletedMissionIds, new[] { missionId }));
        }

        public static SaveGameData SetActiveMission(
            SaveGameData saveGameData,
            string currentMissionId,
            string currentMissionStageId)
        {
            return new SaveGameData(
                currentMissionId: currentMissionId,
                currentMissionStageId: currentMissionStageId,
                lastSceneName: saveGameData.LastSceneName,
                lastSpawnPointId: saveGameData.LastSpawnPointId,
                cash: saveGameData.Cash,
                reputation: saveGameData.Reputation,
                heatLevel: saveGameData.HeatLevel,
                completedMissionIds: saveGameData.CompletedMissionIds,
                unlockedDistrictIds: saveGameData.UnlockedDistrictIds,
                unlockedActivityIds: saveGameData.UnlockedActivityIds,
                currentChapterId: saveGameData.CurrentChapterId,
                completedChapterIds: saveGameData.CompletedChapterIds,
                currentActivityId: saveGameData.CurrentActivityId,
                completedActivityIds: saveGameData.CompletedActivityIds);
        }

        public static SaveGameData SetSceneLocation(
            SaveGameData saveGameData,
            string sceneName,
            string spawnPointId)
        {
            return Copy(
                saveGameData,
                lastSceneName: sceneName,
                lastSpawnPointId: spawnPointId);
        }

        public static SaveGameData SetHeatLevel(SaveGameData saveGameData, int heatLevel)
        {
            return Copy(saveGameData, heatLevel: heatLevel);
        }

        public static SaveGameData SetActiveChapter(SaveGameData saveGameData, string currentChapterId)
        {
            return new SaveGameData(
                currentMissionId: saveGameData.CurrentMissionId,
                currentMissionStageId: saveGameData.CurrentMissionStageId,
                lastSceneName: saveGameData.LastSceneName,
                lastSpawnPointId: saveGameData.LastSpawnPointId,
                cash: saveGameData.Cash,
                reputation: saveGameData.Reputation,
                heatLevel: saveGameData.HeatLevel,
                completedMissionIds: saveGameData.CompletedMissionIds,
                unlockedDistrictIds: saveGameData.UnlockedDistrictIds,
                unlockedActivityIds: saveGameData.UnlockedActivityIds,
                currentChapterId: currentChapterId,
                completedChapterIds: saveGameData.CompletedChapterIds,
                currentActivityId: saveGameData.CurrentActivityId,
                completedActivityIds: saveGameData.CompletedActivityIds);
        }

        public static SaveGameData MarkChapterCompleted(SaveGameData saveGameData, string chapterId)
        {
            return Copy(
                saveGameData,
                completedChapterIds: MergeDistinct(saveGameData.CompletedChapterIds, new[] { chapterId }));
        }

        public static SaveGameData SetActiveActivity(SaveGameData saveGameData, string currentActivityId)
        {
            return new SaveGameData(
                currentMissionId: saveGameData.CurrentMissionId,
                currentMissionStageId: saveGameData.CurrentMissionStageId,
                lastSceneName: saveGameData.LastSceneName,
                lastSpawnPointId: saveGameData.LastSpawnPointId,
                cash: saveGameData.Cash,
                reputation: saveGameData.Reputation,
                heatLevel: saveGameData.HeatLevel,
                completedMissionIds: saveGameData.CompletedMissionIds,
                unlockedDistrictIds: saveGameData.UnlockedDistrictIds,
                unlockedActivityIds: saveGameData.UnlockedActivityIds,
                currentChapterId: saveGameData.CurrentChapterId,
                completedChapterIds: saveGameData.CompletedChapterIds,
                currentActivityId: currentActivityId,
                completedActivityIds: saveGameData.CompletedActivityIds);
        }

        public static SaveGameData MarkActivityCompleted(SaveGameData saveGameData, string activityId)
        {
            return Copy(
                saveGameData,
                completedActivityIds: MergeDistinct(saveGameData.CompletedActivityIds, new[] { activityId }));
        }

        private static SaveGameData Copy(
            SaveGameData source,
            string currentMissionId = null,
            string currentMissionStageId = null,
            string lastSceneName = null,
            string lastSpawnPointId = null,
            int? cash = null,
            int? reputation = null,
            int? heatLevel = null,
            IReadOnlyList<string> completedMissionIds = null,
            IReadOnlyList<string> unlockedDistrictIds = null,
            IReadOnlyList<string> unlockedActivityIds = null,
            string currentChapterId = null,
            IReadOnlyList<string> completedChapterIds = null,
            string currentActivityId = null,
            IReadOnlyList<string> completedActivityIds = null)
        {
            return new SaveGameData(
                currentMissionId: currentMissionId ?? source.CurrentMissionId,
                currentMissionStageId: currentMissionStageId ?? source.CurrentMissionStageId,
                lastSceneName: lastSceneName ?? source.LastSceneName,
                lastSpawnPointId: lastSpawnPointId ?? source.LastSpawnPointId,
                cash: cash ?? source.Cash,
                reputation: reputation ?? source.Reputation,
                heatLevel: heatLevel ?? source.HeatLevel,
                completedMissionIds: completedMissionIds ?? source.CompletedMissionIds,
                unlockedDistrictIds: unlockedDistrictIds ?? source.UnlockedDistrictIds,
                unlockedActivityIds: unlockedActivityIds ?? source.UnlockedActivityIds,
                currentChapterId: currentChapterId ?? source.CurrentChapterId,
                completedChapterIds: completedChapterIds ?? source.CompletedChapterIds,
                currentActivityId: currentActivityId ?? source.CurrentActivityId,
                completedActivityIds: completedActivityIds ?? source.CompletedActivityIds);
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
