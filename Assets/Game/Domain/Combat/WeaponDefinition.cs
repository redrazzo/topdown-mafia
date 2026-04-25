namespace MafiaTopDown.Gameplay.Domain.Combat
{
    public sealed class WeaponDefinition
    {
        public WeaponDefinition(string id, string displayName, DamageProfile primaryDamage, float cooldownSeconds)
        {
            Id = id;
            DisplayName = displayName;
            PrimaryDamage = primaryDamage;
            CooldownSeconds = cooldownSeconds;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public DamageProfile PrimaryDamage { get; }

        public float CooldownSeconds { get; }
    }
}
