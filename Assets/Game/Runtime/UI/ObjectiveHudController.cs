using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public sealed class ObjectiveHudController : MonoBehaviour
    {
        [SerializeField] private SimpleObjectiveSystem objectiveSystem;
        [SerializeField] private SaveGameFileService? saveGameFileService;
        [SerializeField] private CampaignDatabaseAsset? campaignDatabase;
        [SerializeField] private Rect objectiveRect = new Rect(24f, 22f, 500f, 74f);

        private void OnGUI()
        {
            if (objectiveSystem == null || string.IsNullOrWhiteSpace(objectiveSystem.CurrentObjectiveTitle))
            {
                return;
            }

            HudStyleUtility.DrawPanel(
                objectiveRect,
                "Case File - " + ResolveHeading(),
                objectiveSystem.CurrentObjectiveTitle);
        }

        private string ResolveHeading()
        {
            if (saveGameFileService == null || campaignDatabase == null)
            {
                return "Campaign";
            }

            var save = saveGameFileService.LoadOrCreateSave();
            var missionTitle = string.Empty;
            var chapterTitle = string.Empty;

            foreach (var mission in campaignDatabase.Missions)
            {
                if (mission == null || mission.MissionId != save.CurrentMissionId)
                {
                    continue;
                }

                missionTitle = mission.ToDefinition().Title;
                break;
            }

            foreach (var chapter in campaignDatabase.Chapters)
            {
                if (chapter == null || chapter.ChapterId != save.CurrentChapterId)
                {
                    continue;
                }

                chapterTitle = chapter.ToDefinition().Title;
                break;
            }

            if (string.IsNullOrWhiteSpace(chapterTitle) && string.IsNullOrWhiteSpace(missionTitle))
            {
                return "Campaign";
            }

            if (string.IsNullOrWhiteSpace(chapterTitle))
            {
                return missionTitle;
            }

            if (string.IsNullOrWhiteSpace(missionTitle))
            {
                return chapterTitle;
            }

            return chapterTitle + " - " + missionTitle;
        }
    }
}
