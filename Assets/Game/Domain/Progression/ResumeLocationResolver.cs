namespace MafiaTopDown.Gameplay.Domain.Progression
{
    public readonly struct ResumeLocation
    {
        public ResumeLocation(string sceneName, string spawnPointId)
        {
            SceneName = sceneName;
            SpawnPointId = spawnPointId;
        }

        public string SceneName { get; }

        public string SpawnPointId { get; }
    }

    public static class ResumeLocationResolver
    {
        public static ResumeLocation Resolve(SaveGameData saveGameData, string fallbackSceneName, string fallbackSpawnPointId)
        {
            var sceneName = string.IsNullOrWhiteSpace(saveGameData.LastSceneName)
                ? fallbackSceneName
                : saveGameData.LastSceneName;
            var spawnPointId = string.IsNullOrWhiteSpace(saveGameData.LastSpawnPointId)
                ? fallbackSpawnPointId
                : saveGameData.LastSpawnPointId;

            if (TryResolveInteriorReturn(sceneName, out var returnSceneName, out var returnSpawnPointId))
            {
                return new ResumeLocation(returnSceneName, returnSpawnPointId);
            }

            return new ResumeLocation(sceneName, spawnPointId);
        }

        private static bool TryResolveInteriorReturn(string sceneName, out string returnSceneName, out string returnSpawnPointId)
        {
            switch (sceneName)
            {
                case "Interior_BackOffice_01":
                    returnSceneName = "District_01";
                    returnSpawnPointId = "ExteriorReturn";
                    return true;
                case "Interior_BusinessBookkeeper_01":
                    returnSceneName = "District_BusinessCore_01";
                    returnSpawnPointId = "FromBookkeeperInterior";
                    return true;
                case "Interior_OldQuarter_Chapel_01":
                    returnSceneName = "District_OldQuarter_01";
                    returnSpawnPointId = "FromChapelInterior";
                    return true;
                case "Interior_RailYard_Garage_01":
                    returnSceneName = "District_RailYard_01";
                    returnSpawnPointId = "FromGarageInterior";
                    return true;
                default:
                    returnSceneName = string.Empty;
                    returnSpawnPointId = string.Empty;
                    return false;
            }
        }
    }
}
