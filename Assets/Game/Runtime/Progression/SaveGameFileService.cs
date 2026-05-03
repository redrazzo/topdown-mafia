using System.IO;
using System.Linq;
using MafiaTopDown.Gameplay.Domain.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Progression
{
    public sealed class SaveGameFileService : MonoBehaviour
    {
        private const string ActiveSlotPlayerPrefsKey = "MafiaTopDown.ActiveSaveSlot";
        private static readonly string[] LaunchDistrictIds = { "docks", "business-core", "old-quarter", "rail-yard" };

        [SerializeField] private string saveFileName = "savegame.json";
        [SerializeField] private string defaultSceneName = "District_01";
        [SerializeField] private string defaultSpawnPointId = "DefaultSpawn";
        [SerializeField] private int slotCount = 3;
        [SerializeField] private int activeSlotIndex = 1;

        public string SavePath => GetSavePath(activeSlotIndex);

        public int ActiveSlotIndex => activeSlotIndex;

        public int SlotCount => Mathf.Max(3, slotCount);

        private void Awake()
        {
            activeSlotIndex = NormalizeSlotIndex(PlayerPrefs.GetInt(ActiveSlotPlayerPrefsKey, activeSlotIndex));
            MigrateLegacySaveIfNeeded();
        }

        public bool SaveExists()
        {
            return SaveExists(activeSlotIndex);
        }

        public bool SaveExists(int slotIndex)
        {
            MigrateLegacySaveIfNeeded();
            return File.Exists(GetSavePath(slotIndex));
        }

        public void SetActiveSlot(int slotIndex)
        {
            activeSlotIndex = NormalizeSlotIndex(slotIndex);
            PlayerPrefs.SetInt(ActiveSlotPlayerPrefsKey, activeSlotIndex);
            PlayerPrefs.Save();
            MigrateLegacySaveIfNeeded();
        }

        public SaveSlotDescriptor[] GetSlotDescriptors()
        {
            MigrateLegacySaveIfNeeded();
            var descriptors = new SaveSlotDescriptor[SlotCount];
            for (var slotIndex = 1; slotIndex <= SlotCount; slotIndex += 1)
            {
                descriptors[slotIndex - 1] = DescribeSlot(slotIndex);
            }

            return descriptors;
        }

        public SaveSlotDescriptor DescribeSlot(int slotIndex)
        {
            slotIndex = NormalizeSlotIndex(slotIndex);
            var path = GetSavePath(slotIndex);
            if (!File.Exists(path))
            {
                return SaveSlotDescriptor.Empty(slotIndex);
            }

            try
            {
                var save = ReadSaveAtPath(path);
                return SaveSlotDescriptor.FromSaveGameData(slotIndex, save, File.GetLastWriteTimeUtc(path).Ticks);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("Save slot " + slotIndex + " could not be described: " + exception.Message);
                return new SaveSlotDescriptor(
                    slotIndex,
                    exists: true,
                    displayName: "Needs Recovery",
                    currentChapterId: string.Empty,
                    currentMissionId: string.Empty,
                    lastSceneName: string.Empty,
                    lastSpawnPointId: string.Empty,
                    cash: 0,
                    heatLevel: 0,
                    lastWriteUtcTicks: File.GetLastWriteTimeUtc(path).Ticks);
            }
        }

        public SaveGameData CreateFreshSave()
        {
            return CreateFreshSave(activeSlotIndex);
        }

        public SaveGameData CreateFreshSave(int slotIndex)
        {
            SetActiveSlot(slotIndex);
            var freshSave = SaveGameData.CreateFreshGame(defaultSceneName, defaultSpawnPointId);
            Save(freshSave);
            return freshSave;
        }

        public void DeleteSave()
        {
            DeleteSave(activeSlotIndex);
        }

        public void DeleteSave(int slotIndex)
        {
            var path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public SaveGameData LoadOrCreateSave()
        {
            return LoadOrCreateSave(activeSlotIndex);
        }

        public SaveGameData LoadOrCreateSave(int slotIndex)
        {
            SetActiveSlot(slotIndex);
            if (!SaveExists())
            {
                return CreateFreshSave();
            }

            try
            {
                var saveGameData = ReadSaveAtPath(SavePath);
                var migratedSave = EnsureLaunchDistrictsUnlocked(saveGameData);
                if (!ReferenceEquals(saveGameData, migratedSave))
                {
                    Save(migratedSave);
                }

                return migratedSave;
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("Save slot " + activeSlotIndex + " was unreadable and will be recovered: " + exception.Message);
                BackupCorruptedSave(SavePath);
                return CreateFreshSave();
            }
        }

        public void UpdateSceneLocation(string sceneName, string spawnPointId)
        {
            var save = LoadOrCreateSave();
            save = SaveGameMutator.SetSceneLocation(save, sceneName, spawnPointId);
            Save(save);
        }

        public void UpdateHeatLevel(int heatLevel)
        {
            var save = LoadOrCreateSave();
            save = SaveGameMutator.SetHeatLevel(save, heatLevel);
            Save(save);
        }

        public void Save(SaveGameData saveGameData)
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            var document = SaveGameDocument.FromSaveGameData(saveGameData);
            var json = JsonUtility.ToJson(document, true);
            File.WriteAllText(SavePath, json);
        }

        private string GetSavePath(int slotIndex)
        {
            slotIndex = NormalizeSlotIndex(slotIndex);
            var extension = Path.GetExtension(saveFileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".json";
            }

            var baseName = Path.GetFileNameWithoutExtension(saveFileName);
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "savegame";
            }

            return Path.Combine(Application.persistentDataPath, baseName + "-slot-" + slotIndex + extension);
        }

        private static SaveGameData EnsureLaunchDistrictsUnlocked(SaveGameData saveGameData)
        {
            if (LaunchDistrictIds.All(saveGameData.UnlockedDistrictIds.Contains))
            {
                return saveGameData;
            }

            return SaveGameMutator.ApplyReward(
                saveGameData,
                new RewardPayload(
                    cashReward: 0,
                    reputationDelta: 0,
                    unlockDistrictIds: LaunchDistrictIds,
                    unlockActivityIds: new string[0]));
        }

        private int NormalizeSlotIndex(int slotIndex)
        {
            return Mathf.Clamp(slotIndex, 1, SlotCount);
        }

        private void MigrateLegacySaveIfNeeded()
        {
            var legacyPath = Path.Combine(Application.persistentDataPath, saveFileName);
            var firstSlotPath = GetSavePath(1);
            if (string.Equals(legacyPath, firstSlotPath, System.StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(legacyPath) ||
                File.Exists(firstSlotPath))
            {
                return;
            }

            Directory.CreateDirectory(Application.persistentDataPath);
            File.Copy(legacyPath, firstSlotPath, overwrite: false);
        }

        private static SaveGameData ReadSaveAtPath(string path)
        {
            var json = File.ReadAllText(path);
            var document = JsonUtility.FromJson<SaveGameDocument>(json);
            if (document == null)
            {
                throw new InvalidDataException("Save document was empty.");
            }

            return document.ToSaveGameData();
        }

        private static void BackupCorruptedSave(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var backupPath = path + ".broken-" + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            File.Copy(path, backupPath, overwrite: true);
        }

        [System.Serializable]
        private sealed class SaveGameDocument
        {
            public string CurrentChapterId = string.Empty;
            public string CurrentMissionId = string.Empty;
            public string CurrentMissionStageId = string.Empty;
            public string CurrentActivityId = string.Empty;
            public string LastSceneName = string.Empty;
            public string LastSpawnPointId = string.Empty;
            public int Cash;
            public int Reputation;
            public int HeatLevel;
            public string[] CompletedChapterIds = new string[0];
            public string[] CompletedMissionIds = new string[0];
            public string[] CompletedActivityIds = new string[0];
            public string[] UnlockedDistrictIds = new string[0];
            public string[] UnlockedActivityIds = new string[0];

            public SaveGameData ToSaveGameData()
            {
                return new SaveGameData(
                    CurrentMissionId,
                    CurrentMissionStageId,
                    LastSceneName,
                    LastSpawnPointId,
                    Cash,
                    Reputation,
                    HeatLevel,
                    CompletedMissionIds ?? new string[0],
                    UnlockedDistrictIds ?? new string[0],
                    UnlockedActivityIds ?? new string[0],
                    CurrentChapterId,
                    CompletedChapterIds ?? new string[0],
                    CurrentActivityId,
                    CompletedActivityIds ?? new string[0]);
            }

            public static SaveGameDocument FromSaveGameData(SaveGameData saveGameData)
            {
                return new SaveGameDocument
                {
                    CurrentChapterId = saveGameData.CurrentChapterId,
                    CurrentMissionId = saveGameData.CurrentMissionId,
                    CurrentMissionStageId = saveGameData.CurrentMissionStageId,
                    CurrentActivityId = saveGameData.CurrentActivityId,
                    LastSceneName = saveGameData.LastSceneName,
                    LastSpawnPointId = saveGameData.LastSpawnPointId,
                    Cash = saveGameData.Cash,
                    Reputation = saveGameData.Reputation,
                    HeatLevel = saveGameData.HeatLevel,
                    CompletedChapterIds = ToArray(saveGameData.CompletedChapterIds),
                    CompletedMissionIds = ToArray(saveGameData.CompletedMissionIds),
                    CompletedActivityIds = ToArray(saveGameData.CompletedActivityIds),
                    UnlockedDistrictIds = ToArray(saveGameData.UnlockedDistrictIds),
                    UnlockedActivityIds = ToArray(saveGameData.UnlockedActivityIds)
                };
            }

            private static string[] ToArray(System.Collections.Generic.IReadOnlyList<string> values)
            {
                var output = new string[values.Count];
                for (var index = 0; index < values.Count; index += 1)
                {
                    output[index] = values[index];
                }

                return output;
            }
        }
    }
}
