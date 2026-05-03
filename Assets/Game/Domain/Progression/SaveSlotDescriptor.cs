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

                var mission = string.IsNullOrWhiteSpace(CurrentMissionId) ? "Free Roam" : HumanizeIdentifier(CurrentMissionId);
                var scene = HumanizeSceneName(LastSceneName);
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
                : HumanizeIdentifier(saveGameData.CurrentChapterId);

            if (!string.IsNullOrWhiteSpace(saveGameData.CurrentMissionId))
            {
                label += " - " + HumanizeIdentifier(saveGameData.CurrentMissionId);
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

        private static string HumanizeSceneName(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return "Unknown District";
            }

            switch (sceneName)
            {
                case "District_01":
                case "District_Docks":
                    return "Docks";
                case "District_BusinessCore_01":
                    return "Business Core";
                case "District_OldQuarter_01":
                    return "Old Quarter";
                case "District_RailYard_01":
                    return "Rail Yard";
                case "Interior_BackOffice_01":
                    return "Back Office";
                default:
                    return HumanizeIdentifier(sceneName);
            }
        }

        private static string HumanizeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return string.Empty;
            }

            var normalized = identifier
                .Replace("mission-", string.Empty)
                .Replace("chapter-", string.Empty)
                .Replace('_', '-');
            var parts = normalized.Split('-');
            for (var index = 0; index < parts.Length; index += 1)
            {
                parts[index] = HumanizePart(parts[index]);
            }

            return string.Join(" ", parts).Trim();
        }

        private static string HumanizePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (string.Equals(value, "i", System.StringComparison.OrdinalIgnoreCase))
            {
                return "I";
            }

            if (int.TryParse(value, out var number))
            {
                return number == 1 ? "I" : number.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return char.ToUpperInvariant(value[0]) + value.Substring(1).ToLowerInvariant();
        }
    }
}
