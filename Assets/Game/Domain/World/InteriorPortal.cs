namespace MafiaTopDown.Gameplay.Domain.World
{
    public sealed class InteriorPortal
    {
        public InteriorPortal(
            string id,
            string exteriorSceneName,
            string exteriorSpawnPointId,
            string interiorSceneName,
            string interiorSpawnPointId)
        {
            Id = id;
            ExteriorSceneName = exteriorSceneName;
            ExteriorSpawnPointId = exteriorSpawnPointId;
            InteriorSceneName = interiorSceneName;
            InteriorSpawnPointId = interiorSpawnPointId;
        }

        public string Id { get; }

        public string ExteriorSceneName { get; }

        public string ExteriorSpawnPointId { get; }

        public string InteriorSceneName { get; }

        public string InteriorSpawnPointId { get; }
    }
}
