namespace MafiaTopDown.Gameplay.Domain.Law
{
    public sealed class HeatState
    {
        public HeatState(int intensity, PoliceResponseTier responseTier)
        {
            Intensity = intensity;
            ResponseTier = responseTier;
        }

        public int Intensity { get; }

        public PoliceResponseTier ResponseTier { get; }

        public static HeatState Cold => new HeatState(0, PoliceResponseTier.None);
    }
}
