using System.Linq;
using MafiaTopDown.Gameplay.Domain.Campaign;
using MafiaTopDown.Gameplay.Domain.Progression;
using MafiaTopDown.Gameplay.Domain.World;
using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Progression
{
    public sealed class CampaignProgressionController : MonoBehaviour
    {
        [SerializeField] private CampaignDatabaseAsset campaignDatabase;
        [SerializeField] private SaveGameFileService saveGameFileService;

        public SaveGameData EnsureChapterStarted(CampaignChapterAsset chapterAsset)
        {
            if (saveGameFileService == null)
            {
                return null;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            if (chapterAsset == null)
            {
                return save;
            }

            if (save.CompletedChapterIds.Contains(chapterAsset.ChapterId))
            {
                return save;
            }

            if (!string.IsNullOrWhiteSpace(save.CurrentMissionId) &&
                string.IsNullOrWhiteSpace(save.CurrentChapterId) &&
                chapterAsset.Missions.Any(mission => mission.MissionId == save.CurrentMissionId))
            {
                save = SaveGameMutator.SetActiveChapter(save, chapterAsset.ChapterId);
                save = ApplyWorldUnlocks(save);
                saveGameFileService.Save(save);
                return save;
            }

            if (save.CurrentChapterId == chapterAsset.ChapterId && !string.IsNullOrWhiteSpace(save.CurrentMissionId))
            {
                return save;
            }

            var chapterDefinition = chapterAsset.ToDefinition();
            var missions = chapterAsset.Missions.Select(mission => mission.ToDefinition());
            save = CampaignChapterProgressionService.StartChapter(save, chapterDefinition, missions);
            save = ApplyWorldUnlocks(save);
            saveGameFileService.Save(save);
            return save;
        }

        public SaveGameData EnsureMissionStarted(MissionDefinitionAsset missionAsset)
        {
            if (saveGameFileService == null)
            {
                return null;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            if (missionAsset == null)
            {
                return save;
            }

            if (save.CompletedMissionIds.Contains(missionAsset.MissionId))
            {
                return save;
            }

            if (save.CurrentMissionId == missionAsset.MissionId)
            {
                return save;
            }

            save = CampaignMissionService.StartMission(save, missionAsset.ToDefinition());
            save = ApplyWorldUnlocks(save);
            saveGameFileService.Save(save);
            return save;
        }

        public SaveGameData CompleteMissionStage(MissionDefinitionAsset missionAsset, string expectedStageId)
        {
            var save = EnsureMissionStarted(missionAsset);
            if (missionAsset == null)
            {
                return save;
            }

            if (save.CurrentMissionId != missionAsset.MissionId)
            {
                return save;
            }

            if (!string.IsNullOrWhiteSpace(expectedStageId) &&
                save.CurrentMissionStageId != expectedStageId)
            {
                return save;
            }

            save = CampaignMissionService.CompleteCurrentStage(save, missionAsset.ToDefinition());
            save = AdvanceCurrentChapterIfNeeded(save, missionAsset.MissionId);
            save = ApplyWorldUnlocks(save);
            saveGameFileService.Save(save);
            return save;
        }

        public string GetCurrentStageId(MissionDefinitionAsset missionAsset)
        {
            if (saveGameFileService == null)
            {
                return null;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            if (missionAsset == null)
            {
                return save.CurrentMissionStageId;
            }

            if (save.CurrentMissionId != missionAsset.MissionId)
            {
                return missionAsset.ToDefinition().StartingStageId;
            }

            return save.CurrentMissionStageId;
        }

        private SaveGameData ApplyWorldUnlocks(SaveGameData save)
        {
            if (campaignDatabase == null)
            {
                return save;
            }

            return WorldUnlockService.ApplyMissionUnlocks(
                save,
                campaignDatabase.Districts.Select(district => district.ToDefinition()),
                campaignDatabase.Activities.Select(activity => activity.ToDefinition()));
        }

        private SaveGameData AdvanceCurrentChapterIfNeeded(SaveGameData save, string completedMissionId)
        {
            if (campaignDatabase == null || string.IsNullOrWhiteSpace(save.CurrentChapterId))
            {
                return save;
            }

            if (!save.CompletedMissionIds.Contains(completedMissionId) || !string.IsNullOrWhiteSpace(save.CurrentMissionId))
            {
                return save;
            }

            var chapterAsset = campaignDatabase.Chapters
                .FirstOrDefault(candidate => candidate.ChapterId == save.CurrentChapterId);
            if (chapterAsset == null)
            {
                return save;
            }

            var nextChapterAsset = chapterAsset.NextChapterAsset;
            return CampaignChapterProgressionService.AdvanceAfterMissionCompletion(
                save,
                chapterAsset.ToDefinition(),
                chapterAsset.Missions.Select(mission => mission.ToDefinition()),
                nextChapterAsset == null ? null : nextChapterAsset.ToDefinition(),
                nextChapterAsset == null ? null : nextChapterAsset.Missions.Select(mission => mission.ToDefinition()));
        }
    }
}
