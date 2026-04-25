namespace MafiaTopDown.Gameplay.Domain.World
{
    public sealed class VehicleArchetype
    {
        public VehicleArchetype(
            string id,
            string displayName,
            float topSpeed,
            float acceleration,
            float braking,
            float handling)
        {
            Id = id;
            DisplayName = displayName;
            TopSpeed = topSpeed;
            Acceleration = acceleration;
            Braking = braking;
            Handling = handling;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public float TopSpeed { get; }

        public float Acceleration { get; }

        public float Braking { get; }

        public float Handling { get; }
    }
}
