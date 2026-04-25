using System.Collections.Generic;

namespace MafiaTopDown.Gameplay.Domain.Progression
{
    public sealed class SaveGameData
    {
        public SaveGameData(
            string currentMissionId,
            string currentMissionStageId,
            string lastSceneName,
            string lastSpawnPointId,
            int cash,
            int reputation,
            int heatLevel,
            IEnumerable<string> completedMissionIds,
            IEnumerable<string> unlockedDistrictIds,
            IEnumerable<string> unlockedActivityIds)
        {
            CurrentMissionId = currentMissionId;
            CurrentMissionStageId = currentMissionStageId;
            LastSceneName = lastSceneName;
            LastSpawnPointId = lastSpawnPointId;
            Cash = cash;
            Reputation = reputation;
            HeatLevel = heatLevel;
            CompletedMissionIds = new List<string>(completedMissionIds);
            UnlockedDistrictIds = new List<string>(unlockedDistrictIds);
            UnlockedActivityIds = new List<string>(unlockedActivityIds);
            CurrentChapterId = null;
            CompletedChapterIds = new List<string>();
            CurrentActivityId = null;
            CompletedActivityIds = new List<string>();
        }

        public SaveGameData(
            string currentMissionId,
            string currentMissionStageId,
            string lastSceneName,
            string lastSpawnPointId,
            int cash,
            int reputation,
            int heatLevel,
            IEnumerable<string> completedMissionIds,
            IEnumerable<string> unlockedDistrictIds,
            IEnumerable<string> unlockedActivityIds,
            string currentChapterId,
            IEnumerable<string> completedChapterIds,
            string currentActivityId = null,
            IEnumerable<string> completedActivityIds = null)
        {
            CurrentMissionId = currentMissionId;
            CurrentMissionStageId = currentMissionStageId;
            LastSceneName = lastSceneName;
            LastSpawnPointId = lastSpawnPointId;
            Cash = cash;
            Reputation = reputation;
            HeatLevel = heatLevel;
            CompletedMissionIds = new List<string>(completedMissionIds);
            UnlockedDistrictIds = new List<string>(unlockedDistrictIds);
            UnlockedActivityIds = new List<string>(unlockedActivityIds);
            CurrentChapterId = currentChapterId;
            CompletedChapterIds = new List<string>(completedChapterIds);
            CurrentActivityId = currentActivityId;
            CompletedActivityIds = new List<string>(completedActivityIds ?? new string[0]);
        }

        public string CurrentMissionId { get; }

        public string CurrentMissionStageId { get; }

        public string LastSceneName { get; }

        public string LastSpawnPointId { get; }

        public int Cash { get; }

        public int Reputation { get; }

        public int HeatLevel { get; }

        public IReadOnlyList<string> CompletedMissionIds { get; }

        public IReadOnlyList<string> UnlockedDistrictIds { get; }

        public IReadOnlyList<string> UnlockedActivityIds { get; }

        public string CurrentChapterId { get; }

        public IReadOnlyList<string> CompletedChapterIds { get; }

        public string CurrentActivityId { get; }

        public IReadOnlyList<string> CompletedActivityIds { get; }

        public static SaveGameData CreateFreshGame(string sceneName, string spawnPointId)
        {
            return new SaveGameData(
                currentMissionId: null,
                currentMissionStageId: null,
                lastSceneName: sceneName,
                lastSpawnPointId: spawnPointId,
                cash: 0,
                reputation: 0,
                heatLevel: 0,
                completedMissionIds: new string[0],
                unlockedDistrictIds: new[] { "docks" },
                unlockedActivityIds: new string[0]);
        }
    }
}
