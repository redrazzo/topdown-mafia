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

            WriteReleaseCandidateFiles(buildDirectory, executablePath, scenes, report);
            Debug.Log("Windows build succeeded: " + executablePath);
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        private static void WriteReleaseCandidateFiles(string buildDirectory, string executablePath, string[] scenes, BuildReport report)
        {
            var manifest = new ReleaseCandidateManifest
            {
                ProductName = PlayerSettings.productName,
                Version = PlayerSettings.bundleVersion,
                UnityVersion = Application.unityVersion,
                BuildUtc = System.DateTime.UtcNow.ToString("O"),
                GitCommit = ResolveGitCommit(),
                Executable = Path.GetFileName(executablePath),
                SceneCount = scenes.Length,
                Scenes = scenes,
                TotalSizeBytes = GetDirectorySize(buildDirectory),
                Result = report.summary.result.ToString(),
                TotalWarnings = report.summary.totalWarnings,
                TotalErrors = report.summary.totalErrors
            };

            var manifestPath = Path.Combine(buildDirectory, "release-manifest.json");
            File.WriteAllText(manifestPath, JsonUtility.ToJson(manifest, true));

            var readmePath = Path.Combine(buildDirectory, "STEAM_RELEASE_README.txt");
            File.WriteAllText(readmePath, BuildReadme(manifest));
            Debug.Log("Release candidate manifest written: " + manifestPath);
        }

        private static string BuildReadme(ReleaseCandidateManifest manifest)
        {
            return
                "Mafia Topdown City - Windows Release Candidate" + System.Environment.NewLine +
                "Version: " + manifest.Version + System.Environment.NewLine +
                "Commit: " + manifest.GitCommit + System.Environment.NewLine +
                "Built UTC: " + manifest.BuildUtc + System.Environment.NewLine +
                "Executable: " + manifest.Executable + System.Environment.NewLine +
                "Scenes: " + manifest.SceneCount + System.Environment.NewLine +
                "Warnings: " + manifest.TotalWarnings + " | Errors: " + manifest.TotalErrors + System.Environment.NewLine +
                System.Environment.NewLine +
                "Pre-upload sanity:" + System.Environment.NewLine +
                "- Launch MafiaTopDown.exe from this folder." + System.Environment.NewLine +
                "- Start a new save slot and confirm slot persistence after relaunch." + System.Environment.NewLine +
                "- Continue an existing slot and verify the player returns to the saved district/spawn." + System.Environment.NewLine +
                "- Open Settings and verify resolution, fullscreen, subtitles, volume, HUD scale, and sensitivity." + System.Environment.NewLine +
                "- Finish Act I golden path and test at least one side job before store upload." + System.Environment.NewLine +
                "- Check persistent Diagnostics/session-*.log for warnings/errors after a play session." + System.Environment.NewLine;
        }

        private static long GetDirectorySize(string directory)
        {
            var total = 0L;
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            for (var index = 0; index < files.Length; index += 1)
            {
                total += new FileInfo(files[index]).Length;
            }

            return total;
        }

        private static string ResolveGitCommit()
        {
            var root = Directory.GetCurrentDirectory();
            var gitDirectory = Path.Combine(root, ".git");
            var headPath = Path.Combine(gitDirectory, "HEAD");
            if (!File.Exists(headPath))
            {
                return "unknown";
            }

            var head = File.ReadAllText(headPath).Trim();
            const string refPrefix = "ref: ";
            if (!head.StartsWith(refPrefix, System.StringComparison.OrdinalIgnoreCase))
            {
                return head;
            }

            var refPath = Path.Combine(gitDirectory, head.Substring(refPrefix.Length).Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(refPath) ? File.ReadAllText(refPath).Trim() : "unknown";
        }

        [System.Serializable]
        private sealed class ReleaseCandidateManifest
        {
            public string ProductName = string.Empty;
            public string Version = string.Empty;
            public string UnityVersion = string.Empty;
            public string BuildUtc = string.Empty;
            public string GitCommit = string.Empty;
            public string Executable = string.Empty;
            public int SceneCount;
            public string[] Scenes = new string[0];
            public long TotalSizeBytes;
            public string Result = string.Empty;
            public int TotalWarnings;
            public int TotalErrors;
        }
    }
}
