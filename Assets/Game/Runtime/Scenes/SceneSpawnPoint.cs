using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class SceneSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string spawnPointId = "DefaultSpawn";

        public string SpawnPointId => spawnPointId;
    }
}
