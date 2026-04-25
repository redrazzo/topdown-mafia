using System;
using System.Collections.Generic;
using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public static class CampaignChapterProgressionService
    {
        public static SaveGameData StartChapter(
            SaveGameData saveGameData,
            CampaignChapterDefinition chapterDefinition,
            IEnumerable<MissionDefinition> missions)
        {
            var missionMap = missions.ToDictionary(mission => mission.Id);
            var firstMissionId = chapterDefinition.MissionIds[0];
            if (!missionMap.TryGetValue(firstMissionId, out var firstMission))
            {
                throw new InvalidOperationException("Chapter starting mission is not available in the mission list.");
            }

            var updated = SaveGameMutator.SetActiveChapter(saveGameData, chapterDefinition.Id);
            return CampaignMissionService.StartMission(updated, firstMission);
        }

        public static SaveGameData AdvanceAfterMissionCompletion(
            SaveGameData saveGameData,
            CampaignChapterDefinition chapterDefinition,
            IEnumerable<MissionDefinition> missions,
            CampaignChapterDefinition nextChapterDefinition = null,
            IEnumerable<MissionDefinition> nextChapterMissions = null)
        {
            var missionMap = missions.ToDictionary(mission => mission.Id);
            var nextMissionId = chapterDefinition.MissionIds
                .FirstOrDefault(missionId => !saveGameData.CompletedMissionIds.Contains(missionId));

            if (string.IsNullOrWhiteSpace(nextMissionId))
            {
                var cleared = SaveGameMutator.SetActiveMission(saveGameData, null, null);
                cleared = SaveGameMutator.MarkChapterCompleted(cleared, chapterDefinition.Id);
                cleared = SaveGameMutator.SetActiveChapter(cleared, null);

                if (nextChapterDefinition == null)
                {
                    return cleared;
                }

                if (nextChapterMissions == null)
                {
                    throw new InvalidOperationException("Next chapter missions must be supplied when a next chapter is provided.");
                }

                return StartChapter(cleared, nextChapterDefinition, nextChapterMissions);
            }

            if (!missionMap.TryGetValue(nextMissionId, out var nextMission))
            {
                throw new InvalidOperationException("Next chapter mission is not available in the mission list.");
            }

            var updated = SaveGameMutator.SetActiveChapter(saveGameData, chapterDefinition.Id);
            return CampaignMissionService.StartMission(updated, nextMission);
        }
    }
}
