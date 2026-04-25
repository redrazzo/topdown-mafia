namespace MafiaTopDown.Gameplay.Domain.Campaign
{
    public sealed class MissionCheckpoint
    {
        public MissionCheckpoint(
            string id,
            string sceneName,
            string spawnPointId,
            string missionStageId)
        {
            Id = id;
            SceneName = sceneName;
            SpawnPointId = spawnPointId;
            MissionStageId = missionStageId;
        }

        public string Id { get; }

        public string SceneName { get; }

        public string SpawnPointId { get; }

        public string MissionStageId { get; }
    }
}
