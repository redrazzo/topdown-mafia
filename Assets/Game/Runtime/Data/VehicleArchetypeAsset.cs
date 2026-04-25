using MafiaTopDown.Gameplay.Domain.World;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Vehicle Archetype", fileName = "VehicleArchetype")]
    public sealed class VehicleArchetypeAsset : ScriptableObject
    {
        [SerializeField] private string archetypeId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private float topSpeed = 18f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float braking = 12f;
        [SerializeField] private float handling = 90f;

        public VehicleArchetype ToDefinition()
        {
            return new VehicleArchetype(archetypeId, displayName, topSpeed, acceleration, braking, handling);
        }
    }
}
