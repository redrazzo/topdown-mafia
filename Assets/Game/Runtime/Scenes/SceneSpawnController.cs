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
            if (saveGameFileService == null || playerController == null)
            {
                return;
            }

            var save = saveGameFileService.LoadOrCreateSave();
            if (save.LastSceneName != SceneManager.GetActiveScene().name)
            {
                return;
            }

            var targetSpawnId = string.IsNullOrWhiteSpace(save.LastSpawnPointId)
                ? defaultSpawnPointId
                : save.LastSpawnPointId;
            var spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.SpawnPointId != targetSpawnId)
                {
                    continue;
                }

                playerController.transform.SetPositionAndRotation(
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation);
                return;
            }
        }
    }
}
