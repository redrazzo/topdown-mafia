using MafiaTopDown.Gameplay.Domain.World;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Landmark Definition", fileName = "LandmarkDefinition")]
    public sealed class LandmarkDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string landmarkId = string.Empty;
        [SerializeField] private string districtId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private LandmarkType landmarkType;

        public LandmarkDefinition ToDefinition()
        {
            return new LandmarkDefinition(landmarkId, districtId, displayName, landmarkType);
        }
    }
}
