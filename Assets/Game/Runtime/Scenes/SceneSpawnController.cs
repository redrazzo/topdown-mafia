using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class SceneSpawnController : MonoBehaviour
    {
        [SerializeField] private SaveGameFileService saveGameFileService = null!;
        [SerializeField] private TopDownPlayerController playerController = null!;
        [SerializeField] private string defaultSpawnPointId = "DefaultSpawn";
        [SerializeField] private float clearanceProbeStep = 0.85f;
        [SerializeField] private int clearanceProbeRings = 3;

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
                ApplySpawnTransform(fallback);
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

                ApplySpawnTransform(spawnPoint);
                return true;
            }

            return false;
        }

        private void ApplySpawnTransform(SceneSpawnPoint spawnPoint)
        {
            var player = playerController;
            if (player == null)
            {
                return;
            }

            var characterController = player.GetComponent<CharacterController>();
            var wasEnabled = characterController != null && characterController.enabled;
            if (wasEnabled && characterController != null)
            {
                characterController.enabled = false;
            }

            var spawnPosition = ResolveClearSpawnPosition(spawnPoint.transform.position, characterController);
            player.transform.SetPositionAndRotation(spawnPosition, spawnPoint.transform.rotation);

            if (wasEnabled && characterController != null)
            {
                characterController.enabled = true;
            }
        }

        private Vector3 ResolveClearSpawnPosition(Vector3 requestedPosition, CharacterController? characterController)
        {
            if (IsSpawnClear(requestedPosition, characterController))
            {
                return requestedPosition;
            }

            var directions = new[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right,
                (Vector3.forward + Vector3.left).normalized,
                (Vector3.forward + Vector3.right).normalized,
                (Vector3.back + Vector3.left).normalized,
                (Vector3.back + Vector3.right).normalized
            };

            for (var ring = 1; ring <= clearanceProbeRings; ring += 1)
            {
                var distance = clearanceProbeStep * ring;
                foreach (var direction in directions)
                {
                    var candidate = requestedPosition + (direction * distance);
                    if (IsSpawnClear(candidate, characterController))
                    {
                        Debug.LogWarning("SceneSpawnController repaired blocked spawn from " + requestedPosition + " to " + candidate + ".");
                        return candidate;
                    }
                }
            }

            Debug.LogWarning("SceneSpawnController could not find a clear spawn near " + requestedPosition + "; using requested position.");
            return requestedPosition;
        }

        private static bool IsSpawnClear(Vector3 position, CharacterController? characterController)
        {
            var radius = characterController != null ? Mathf.Max(0.2f, characterController.radius) : 0.35f;
            var height = characterController != null ? Mathf.Max(1f, characterController.height) : 1.8f;
            var bottom = position + Vector3.up * (radius + 0.08f);
            var top = position + Vector3.up * Mathf.Max(radius + 0.12f, height - radius);
            return !Physics.CheckCapsule(bottom, top, radius, ~0, QueryTriggerInteraction.Ignore);
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
