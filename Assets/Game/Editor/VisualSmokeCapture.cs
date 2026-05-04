#nullable enable

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MafiaTopDown.Editor
{
    public static class VisualSmokeCapture
    {
        private static readonly string[] DistrictScenePaths =
        {
            "Assets/Game/Scenes/District_01.unity",
            "Assets/Game/Scenes/District_BusinessCore_01.unity",
            "Assets/Game/Scenes/District_OldQuarter_01.unity",
            "Assets/Game/Scenes/District_RailYard_01.unity"
        };

        public static void RunBatchDistrictCapture()
        {
            var outputRoot = Path.Combine(Application.dataPath, "..", "Builds", "Windows", "VisualSmoke");
            Directory.CreateDirectory(outputRoot);

            foreach (var scenePath in DistrictScenePaths)
            {
                var scene = EditorSceneManager.OpenScene(scenePath);
                var camera = Camera.main != null
                    ? Camera.main
                    : UnityEngine.Object.FindFirstObjectByType<Camera>();
                if (camera == null)
                {
                    throw new System.InvalidOperationException("No camera found in " + scenePath + ".");
                }

                ApplyGameplayCameraPose(camera);
                var outputPath = Path.Combine(outputRoot, scene.name + ".png");
                CaptureCamera(camera, outputPath);
                Debug.Log("Visual smoke captured: " + outputPath);
            }
        }

        public static void RunBatchBootCapture()
        {
            var outputRoot = Path.Combine(Application.dataPath, "..", "Builds", "Windows", "VisualSmoke");
            Directory.CreateDirectory(outputRoot);

            var scene = EditorSceneManager.OpenScene("Assets/Game/Scenes/Boot.unity");
            var camera = Camera.main != null
                ? Camera.main
                : UnityEngine.Object.FindFirstObjectByType<Camera>();
            if (camera == null)
            {
                throw new System.InvalidOperationException("No camera found in " + scene.path + ".");
            }

            var outputPath = Path.Combine(outputRoot, scene.name + ".png");
            CaptureCamera(camera, outputPath);
            Debug.Log("Boot visual smoke captured: " + outputPath);
        }

        private static void CaptureCamera(Camera camera, string outputPath)
        {
            const int width = 1550;
            const int height = 902;

            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 2
            };
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            try
            {
                camera.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;
                camera.Render();
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                File.WriteAllBytes(outputPath, EncodeTextureToPng(texture));
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(renderTexture);
            }
        }

        private static void ApplyGameplayCameraPose(Camera camera)
        {
            var player = GameObject.Find("Player");
            if (player == null)
            {
                return;
            }

            camera.fieldOfView = 38f;
            camera.transform.position = player.transform.position + new Vector3(0.2f, 4.85f, -4.2f);
            camera.transform.rotation = Quaternion.Euler(58f, 8f, 0f);
        }

        private static byte[] EncodeTextureToPng(Texture2D texture)
        {
            var instanceMethod = typeof(Texture2D).GetMethod("EncodeToPNG", System.Type.EmptyTypes);
            if (instanceMethod?.Invoke(texture, System.Array.Empty<object>()) is byte[] instanceBytes)
            {
                return instanceBytes;
            }

            System.Type? imageConversionType = null;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                imageConversionType = assembly.GetType("UnityEngine.ImageConversion");
                if (imageConversionType != null)
                {
                    break;
                }
            }

            var method = imageConversionType?.GetMethod("EncodeToPNG", new[] { typeof(Texture2D) });
            if (method == null)
            {
                throw new System.InvalidOperationException("UnityEngine.ImageConversion.EncodeToPNG is unavailable.");
            }

            if (method.Invoke(null, new object[] { texture }) is byte[] pngBytes)
            {
                return pngBytes;
            }

            throw new System.InvalidOperationException("UnityEngine.ImageConversion.EncodeToPNG returned no data.");
        }
    }
}
