using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class SceneSpawnController : MonoBehaviour
    {
        [SerializeField] private SaveGameFileService saveGameFileService;
        [SerializeField] private TopDownPlayerController playerController;
        [SerializeField] private string defaultSpawnPointId = "DefaultSpawn";

        private void Start()
        {
            ApplySpawnFromSave();
        }

        public void ApplySpawnFromSave()
        {
            ResolveDependencies();
            if (saveGameFileService == null || playerController == null)
            {
                return;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            var activeSceneName = SceneManager.GetActiveScene().name;
            var targetSpawnId = save.LastSceneName == activeSceneName && !string.IsNullOrWhiteSpace(save.LastSpawnPointId)
                ? save.LastSpawnPointId
                : defaultSpawnPointId;
            targetSpawnId = string.IsNullOrWhiteSpace(targetSpawnId)
                ? defaultSpawnPointId
                : targetSpawnId;
            var spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            if (TryApplySpawn(spawnPoints, targetSpawnId))
            {
                RepairSavedLocationIfNeeded(save.LastSceneName, save.LastSpawnPointId, activeSceneName, targetSpawnId);
                return;
            }

            if (TryApplySpawn(spawnPoints, defaultSpawnPointId))
            {
                RepairSavedLocationIfNeeded(save.LastSceneName, save.LastSpawnPointId, activeSceneName, defaultSpawnPointId);
                return;
            }

            if (spawnPoints.Length > 0)
            {
                var fallback = spawnPoints[0];
                playerController.transform.SetPositionAndRotation(fallback.transform.position, fallback.transform.rotation);
                RepairSavedLocationIfNeeded(save.LastSceneName, save.LastSpawnPointId, activeSceneName, fallback.SpawnPointId);
                Debug.LogWarning("SceneSpawnController used first available spawn because no requested/default spawn point was found in " + activeSceneName + ".");
            }
        }

        private bool TryApplySpawn(SceneSpawnPoint[] spawnPoints, string spawnPointId)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.SpawnPointId != spawnPointId)
                {
                    continue;
                }

                playerController.transform.SetPositionAndRotation(
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation);
                return true;
            }

            return false;
        }

        private void ResolveDependencies()
        {
            saveGameFileService ??= FindFirstObjectByType<SaveGameFileService>();
            playerController ??= FindFirstObjectByType<TopDownPlayerController>();
        }

        private void RepairSavedLocationIfNeeded(string savedSceneName, string savedSpawnPointId, string activeSceneName, string resolvedSpawnPointId)
        {
            if (saveGameFileService == null)
            {
                return;
            }

            if (savedSceneName == activeSceneName && savedSpawnPointId == resolvedSpawnPointId)
            {
                return;
            }

            saveGameFileService.UpdateSceneLocation(activeSceneName, resolvedSpawnPointId);
        }
    }
}
