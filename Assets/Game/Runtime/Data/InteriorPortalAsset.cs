using MafiaTopDown.Gameplay.Domain.World;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Interior Portal", fileName = "InteriorPortal")]
    public sealed class InteriorPortalAsset : ScriptableObject
    {
        [SerializeField] private string portalId = string.Empty;
        [SerializeField] private string exteriorSceneName = string.Empty;
        [SerializeField] private string exteriorSpawnPointId = string.Empty;
        [SerializeField] private string interiorSceneName = string.Empty;
        [SerializeField] private string interiorSpawnPointId = string.Empty;

        public InteriorPortal ToDefinition()
        {
            return new InteriorPortal(portalId, exteriorSceneName, exteriorSpawnPointId, interiorSceneName, interiorSpawnPointId);
        }
    }
}
