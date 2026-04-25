using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MafiaTopDown.Editor
{
    public static class WindowsBuildAutomation
    {
        [MenuItem("MafiaTopDown/Build/Windows Prototype")]
        public static void BuildWindowsPrototypeFromMenu()
        {
            RunBatchWindowsBuild();
        }

        public static void RunBatchWindowsBuild()
        {
            var buildDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Windows");
            Directory.CreateDirectory(buildDirectory);

            var executablePath = Path.Combine(buildDirectory, "MafiaTopDown.exe");
            var scenes = new string[EditorBuildSettings.scenes.Length];
            for (var index = 0; index < EditorBuildSettings.scenes.Length; index += 1)
            {
                scenes[index] = EditorBuildSettings.scenes[index].path;
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = executablePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Windows build failed with result: " + report.summary.result);
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }

                return;
            }

            Debug.Log("Windows build succeeded: " + executablePath);
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
    }
}
