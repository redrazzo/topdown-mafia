using MafiaTopDown.Gameplay.Domain.World;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Activity Definition", fileName = "ActivityDefinition")]
    public sealed class ActivityDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string activityId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private ActivityType activityType;
        [SerializeField] private string districtId = string.Empty;
        [SerializeField] private int rewardCash;
        [SerializeField] private bool repeatable = true;
        [SerializeField] private string unlockMissionId = string.Empty;

        public string ActivityId => activityId;

        public string DisplayName => displayName;

        public ActivityDefinition ToDefinition()
        {
            return new ActivityDefinition(activityId, displayName, activityType, districtId, rewardCash, repeatable, unlockMissionId);
        }
    }
}
