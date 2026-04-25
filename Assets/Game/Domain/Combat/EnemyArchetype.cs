namespace MafiaTopDown.Gameplay.Domain.Combat
{
    public sealed class EnemyArchetype
    {
        public EnemyArchetype(
            string id,
            string displayName,
            int maximumHealth,
            int contactDamage,
            float moveSpeed,
            float aggroRange)
        {
            Id = id;
            DisplayName = displayName;
            MaximumHealth = maximumHealth;
            ContactDamage = contactDamage;
            MoveSpeed = moveSpeed;
            AggroRange = aggroRange;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public int MaximumHealth { get; }

        public int ContactDamage { get; }

        public float MoveSpeed { get; }

        public float AggroRange { get; }
    }
}
