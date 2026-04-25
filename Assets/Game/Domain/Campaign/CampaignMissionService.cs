using System;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public static class CampaignMissionService
    {
        public static SaveGameData StartMission(SaveGameData saveGameData, MissionDefinition missionDefinition)
        {
            var updated = SaveGameMutator.SetActiveMission(
                saveGameData,
                missionDefinition.Id,
                missionDefinition.StartingStageId);

            return ApplyCheckpointForStage(updated, missionDefinition, missionDefinition.StartingStageId);
        }

        public static SaveGameData CompleteCurrentStage(SaveGameData saveGameData, MissionDefinition missionDefinition)
        {
            if (!string.Equals(saveGameData.CurrentMissionId, missionDefinition.Id, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The supplied save does not have the requested mission active.");
            }

            var stageId = string.IsNullOrWhiteSpace(saveGameData.CurrentMissionStageId)
                ? missionDefinition.StartingStageId
                : saveGameData.CurrentMissionStageId;
            var currentStage = missionDefinition.GetStage(stageId);

            if (string.IsNullOrWhiteSpace(currentStage.NextStageId))
            {
                var completed = SaveGameMutator.SetActiveMission(saveGameData, null, null);
                completed = SaveGameMutator.MarkMissionCompleted(completed, missionDefinition.Id);
                return SaveGameMutator.ApplyReward(completed, missionDefinition.CompletionReward);
            }

            var advanced = SaveGameMutator.SetActiveMission(
                saveGameData,
                missionDefinition.Id,
                currentStage.NextStageId);

            return ApplyCheckpointForStage(advanced, missionDefinition, currentStage.NextStageId);
        }

        private static SaveGameData ApplyCheckpointForStage(
            SaveGameData saveGameData,
            MissionDefinition missionDefinition,
            string stageId)
        {
            var checkpoint = missionDefinition.GetCheckpointForStage(stageId);
            if (checkpoint == null)
            {
                return saveGameData;
            }

            return SaveGameMutator.SetSceneLocation(saveGameData, checkpoint.SceneName, checkpoint.SpawnPointId);
        }
    }
}
