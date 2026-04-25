using System.IO;
using MafiaTopDown.Gameplay.Domain.Progression;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Progression
{
    public sealed class SaveGameFileService : MonoBehaviour
    {
        [SerializeField] private string saveFileName = "savegame.json";
        [SerializeField] private string defaultSceneName = "District_01";
        [SerializeField] private string defaultSpawnPointId = "DefaultSpawn";

        public string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

        public bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        public SaveGameData CreateFreshSave()
        {
            var freshSave = SaveGameData.CreateFreshGame(defaultSceneName, defaultSpawnPointId);
            Save(freshSave);
            return freshSave;
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }

        public SaveGameData LoadOrCreateSave()
        {
            if (!SaveExists())
            {
                return CreateFreshSave();
            }

            var json = File.ReadAllText(SavePath);
            var document = JsonUtility.FromJson<SaveGameDocument>(json);
            return document.ToSaveGameData();
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
            var document = SaveGameDocument.FromSaveGameData(saveGameData);
            var json = JsonUtility.ToJson(document, true);
            File.WriteAllText(SavePath, json);
        }

        [System.Serializable]
        private sealed class SaveGameDocument
        {
            public string CurrentChapterId;
            public string CurrentMissionId;
            public string CurrentMissionStageId;
            public string CurrentActivityId;
            public string LastSceneName;
            public string LastSpawnPointId;
            public int Cash;
            public int Reputation;
            public int HeatLevel;
            public string[] CompletedChapterIds;
            public string[] CompletedMissionIds;
            public string[] CompletedActivityIds;
            public string[] UnlockedDistrictIds;
            public string[] UnlockedActivityIds;

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
