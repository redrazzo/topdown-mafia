using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;

namespace MafiaTopDown.Gameplay.Domain.World
{
    public static class SideJobProgressionService
    {
        public static SaveGameData StartActivity(SaveGameData saveGameData, ActivityDefinition activityDefinition)
        {
            if (activityDefinition == null)
            {
                return saveGameData;
            }

            if (!saveGameData.UnlockedActivityIds.Contains(activityDefinition.Id))
            {
                return saveGameData;
            }

            if (!string.IsNullOrWhiteSpace(saveGameData.CurrentMissionId))
            {
                return saveGameData;
            }

            return SaveGameMutator.SetActiveActivity(saveGameData, activityDefinition.Id);
        }

        public static SaveGameData CompleteActivity(SaveGameData saveGameData, ActivityDefinition activityDefinition)
        {
            if (activityDefinition == null || saveGameData.CurrentActivityId != activityDefinition.Id)
            {
                return saveGameData;
            }

            var updated = SaveGameMutator.SetActiveActivity(saveGameData, null);
            updated = SaveGameMutator.MarkActivityCompleted(updated, activityDefinition.Id);
            return SaveGameMutator.ApplyReward(
                updated,
                new RewardPayload(activityDefinition.RewardCash, 0, new string[0], new string[0]));
        }
    }
}
