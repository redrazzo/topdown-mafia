using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace MafiaTopDown.Editor
{
    public static class ProjectBootstrapper
    {
        private static readonly Queue<string> PendingPackages = new Queue<string>();
        private static AddRequest _currentRequest;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            EditorApplication.delayCall -= EnsureDefaultStructureOnLoad;
            EditorApplication.delayCall += EnsureDefaultStructureOnLoad;
        }

        [MenuItem("MafiaTopDown/Setup/Install Recommended Packages")]
        public static void InstallRecommendedPackages()
        {
            PendingPackages.Clear();
            PendingPackages.Enqueue("com.unity.inputsystem");
            PendingPackages.Enqueue("com.unity.ai.navigation");
            PendingPackages.Enqueue("com.unity.cinemachine");
            PendingPackages.Enqueue("com.unity.test-framework");
            PendingPackages.Enqueue("com.unity.render-pipelines.universal");

            EditorApplication.update -= ProcessPackageQueue;
            EditorApplication.update += ProcessPackageQueue;
            Debug.Log("Queued package installation for Input System, AI Navigation, Cinemachine, Test Framework, and URP.");
        }

        [MenuItem("MafiaTopDown/Setup/Create Starter Scenes")]
        public static void CreateStarterScenes()
        {
            EnsureFolder("Assets/Game/Scenes");

            CreateSceneAsset("Assets/Game/Scenes/Boot.unity", "Boot");
            CreateSceneAsset("Assets/Game/Scenes/District_01.unity", "District_01");
            CreateSceneAsset("Assets/Game/Scenes/Interior_BackOffice_01.unity", "Interior_BackOffice_01");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Starter scenes created under Assets/Game/Scenes.");
        }

        [MenuItem("MafiaTopDown/Setup/Create Project Folders")]
        public static void CreateProjectFolders()
        {
            EnsureFolder("Assets/Game/Art");
            EnsureFolder("Assets/Game/Audio");
            EnsureFolder("Assets/Game/Materials");
            EnsureFolder("Assets/Game/Prefabs");
            EnsureFolder("Assets/Game/Scenes");
            EnsureFolder("Assets/Game/Lighting");
            EnsureFolder("Assets/Game/UI");
            EnsureFolder("Assets/Game/Runtime");

            AssetDatabase.Refresh();
            Debug.Log("Project folders ensured.");
        }

        public static void RunBatchSceneBootstrap()
        {
            CreateProjectFolders();
            CreateStarterScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        public static void RunBatchPackageBootstrap()
        {
            var packages = new[]
            {
                "com.unity.inputsystem",
                "com.unity.ai.navigation",
                "com.unity.cinemachine",
                "com.unity.test-framework",
                "com.unity.render-pipelines.universal"
            };

            foreach (var packageId in packages)
            {
                var request = Client.Add(packageId);
                while (!request.IsCompleted)
                {
                    Thread.Sleep(100);
                }

                if (request.Status == StatusCode.Failure)
                {
                    Debug.LogError("Package install failed: " + packageId + " :: " + request.Error.message);
                    if (Application.isBatchMode)
                    {
                        EditorApplication.Exit(1);
                    }

                    return;
                }

                Debug.Log("Installed or resolved package: " + packageId);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        private static void EnsureDefaultStructureOnLoad()
        {
            if (Application.isBatchMode)
            {
                return;
            }

            var bootSceneExists = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Game/Scenes/Boot.unity") != null;
            var districtSceneExists = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Game/Scenes/District_01.unity") != null;
            var interiorSceneExists = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Game/Scenes/Interior_BackOffice_01.unity") != null;

            if (bootSceneExists && districtSceneExists && interiorSceneExists)
            {
                return;
            }

            CreateProjectFolders();
            CreateStarterScenes();
            Debug.Log("MafiaTopDown bootstrap created the default folders and starter scenes.");
        }

        private static void ProcessPackageQueue()
        {
            if (_currentRequest != null)
            {
                if (!_currentRequest.IsCompleted)
                {
                    return;
                }

                if (_currentRequest.Status == StatusCode.Failure)
                {
                    Debug.LogError("Package install failed: " + _currentRequest.Error.message);
                }

                _currentRequest = null;
            }

            if (PendingPackages.Count == 0)
            {
                EditorApplication.update -= ProcessPackageQueue;
                Debug.Log("Package queue completed.");
                return;
            }

            _currentRequest = Client.Add(PendingPackages.Dequeue());
        }

        private static void EnsureFolder(string assetPath)
        {
            var parts = assetPath.Split('/');
            var current = parts[0];

            for (var index = 1; index < parts.Length; index += 1)
            {
                var next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void CreateSceneAsset(string path, string sceneName)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;
            EditorSceneManager.SaveScene(scene, path);
        }
    }
}
