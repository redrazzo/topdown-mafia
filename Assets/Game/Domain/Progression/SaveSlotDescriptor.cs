namespace MafiaTopDown.Gameplay.Domain.Progression
{
    public sealed class SaveSlotDescriptor
    {
        public SaveSlotDescriptor(
            int slotIndex,
            bool exists,
            string displayName,
            string currentChapterId,
            string currentMissionId,
            string lastSceneName,
            string lastSpawnPointId,
            int cash,
            int heatLevel,
            long lastWriteUtcTicks)
        {
            SlotIndex = slotIndex;
            Exists = exists;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Empty Slot" : displayName;
            CurrentChapterId = currentChapterId ?? string.Empty;
            CurrentMissionId = currentMissionId ?? string.Empty;
            LastSceneName = lastSceneName ?? string.Empty;
            LastSpawnPointId = lastSpawnPointId ?? string.Empty;
            Cash = cash;
            HeatLevel = heatLevel;
            LastWriteUtcTicks = lastWriteUtcTicks;
        }

        public int SlotIndex { get; }

        public bool Exists { get; }

        public string DisplayName { get; }

        public string CurrentChapterId { get; }

        public string CurrentMissionId { get; }

        public string LastSceneName { get; }

        public string LastSpawnPointId { get; }

        public int Cash { get; }

        public int HeatLevel { get; }

        public long LastWriteUtcTicks { get; }

        public string Summary
        {
            get
            {
                if (!Exists)
                {
                    return "No save data yet";
                }

                var mission = string.IsNullOrWhiteSpace(CurrentMissionId) ? "Free roam" : CurrentMissionId;
                var scene = string.IsNullOrWhiteSpace(LastSceneName) ? "Unknown district" : LastSceneName;
                return mission + " | " + scene + " | $" + Cash + " | Heat " + HeatLevel;
            }
        }

        public static SaveSlotDescriptor Empty(int slotIndex)
        {
            return new SaveSlotDescriptor(
                slotIndex,
                exists: false,
                displayName: "Empty Slot",
                currentChapterId: string.Empty,
                currentMissionId: string.Empty,
                lastSceneName: string.Empty,
                lastSpawnPointId: string.Empty,
                cash: 0,
                heatLevel: 0,
                lastWriteUtcTicks: 0);
        }

        public static SaveSlotDescriptor FromSaveGameData(int slotIndex, SaveGameData saveGameData, long lastWriteUtcTicks)
        {
            var label = string.IsNullOrWhiteSpace(saveGameData.CurrentChapterId)
                ? "Open City"
                : saveGameData.CurrentChapterId;

            if (!string.IsNullOrWhiteSpace(saveGameData.CurrentMissionId))
            {
                label += " - " + saveGameData.CurrentMissionId;
            }

            return new SaveSlotDescriptor(
                slotIndex,
                exists: true,
                displayName: label,
                currentChapterId: saveGameData.CurrentChapterId,
                currentMissionId: saveGameData.CurrentMissionId,
                lastSceneName: saveGameData.LastSceneName,
                lastSpawnPointId: saveGameData.LastSpawnPointId,
                cash: saveGameData.Cash,
                heatLevel: saveGameData.HeatLevel,
                lastWriteUtcTicks);
        }
    }
}
