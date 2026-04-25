namespace MafiaTopDown.Gameplay.Domain.Combat
{
    public sealed class DamageProfile
    {
        public DamageProfile(int baseDamage, int heatGenerated, float rangeMeters)
        {
            BaseDamage = baseDamage;
            HeatGenerated = heatGenerated;
            RangeMeters = rangeMeters;
        }

        public int BaseDamage { get; }

        public int HeatGenerated { get; }

        public float RangeMeters { get; }
    }
}
