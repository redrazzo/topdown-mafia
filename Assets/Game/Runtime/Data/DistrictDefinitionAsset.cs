using MafiaTopDown.Gameplay.Domain.World;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/District Definition", fileName = "DistrictDefinition")]
    public sealed class DistrictDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string districtId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private string sceneName = string.Empty;
        [SerializeField] private DistrictTheme theme;
        [SerializeField] private bool unlockedByDefault;
        [SerializeField] private string unlockMissionId = string.Empty;
        [SerializeField] private string[] landmarkIds = new string[0];
        [SerializeField] private string[] activityIds = new string[0];

        public DistrictDefinition ToDefinition()
        {
            return new DistrictDefinition(
                districtId,
                displayName,
                sceneName,
                theme,
                unlockedByDefault,
                unlockMissionId,
                landmarkIds,
                activityIds);
        }
    }
}
