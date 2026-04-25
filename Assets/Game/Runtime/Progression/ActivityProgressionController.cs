using System.Linq;
using MafiaTopDown.Gameplay.Domain.World;
using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Progression
{
    public sealed class ActivityProgressionController : MonoBehaviour
    {
        [SerializeField] private CampaignDatabaseAsset campaignDatabase;
        [SerializeField] private SaveGameFileService saveGameFileService;

        public string CurrentActivityId
        {
            get
            {
                var save = saveGameFileService == null ? null : saveGameFileService.LoadOrCreateSave();
                return save?.CurrentActivityId;
            }
        }

        public string CurrentActivityDisplayName
        {
            get
            {
                var activityId = CurrentActivityId;
                if (string.IsNullOrWhiteSpace(activityId) || campaignDatabase == null)
                {
                    return string.Empty;
                }

                var activity = campaignDatabase.Activities.FirstOrDefault(candidate => candidate.ActivityId == activityId);
                return activity == null ? string.Empty : activity.DisplayName;
            }
        }

        public bool CanStart(ActivityDefinitionAsset activityAsset)
        {
            if (activityAsset == null || saveGameFileService == null)
            {
                return false;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            if (!string.IsNullOrWhiteSpace(save.CurrentActivityId))
            {
                return false;
            }

            return save.UnlockedActivityIds.Contains(activityAsset.ActivityId);
        }

        public bool CanComplete(ActivityDefinitionAsset activityAsset)
        {
            if (activityAsset == null || saveGameFileService == null)
            {
                return false;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            return save.CurrentActivityId == activityAsset.ActivityId;
        }

        public void StartActivity(ActivityDefinitionAsset activityAsset)
        {
            if (!CanStart(activityAsset))
            {
                return;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            save = SideJobProgressionService.StartActivity(save, activityAsset.ToDefinition());
            saveGameFileService.Save(save);
        }

        public void CompleteActivity(ActivityDefinitionAsset activityAsset)
        {
            if (!CanComplete(activityAsset))
            {
                return;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            save = SideJobProgressionService.CompleteActivity(save, activityAsset.ToDefinition());
            saveGameFileService.Save(save);
        }
    }
}
