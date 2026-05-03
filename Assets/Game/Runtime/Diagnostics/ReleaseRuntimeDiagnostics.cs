using System.IO;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Diagnostics
{
    public static class ReleaseRuntimeDiagnostics
    {
        private static bool _initialized;
        private static string? _sessionLogPath;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            var logDirectory = Path.Combine(Application.persistentDataPath, "Diagnostics");
            Directory.CreateDirectory(logDirectory);
            _sessionLogPath = Path.Combine(logDirectory, "session-" + System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".log");
            AppendLine("Session started UTC " + System.DateTime.UtcNow.ToString("O"));
            AppendLine("Version " + Application.version + " | Unity " + Application.unityVersion + " | Platform " + Application.platform);
            Application.logMessageReceived += HandleLogMessage;
        }

        private static void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning && type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
            {
                return;
            }

            AppendLine("[" + type + "] " + condition);
            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                AppendLine(stackTrace);
            }
        }

        private static void AppendLine(string line)
        {
            if (string.IsNullOrWhiteSpace(_sessionLogPath))
            {
                return;
            }

            try
            {
                File.AppendAllText(_sessionLogPath, line + System.Environment.NewLine);
            }
            catch (IOException)
            {
                // Diagnostics must never break gameplay or boot.
            }
            catch (System.UnauthorizedAccessException)
            {
            }
        }
    }
}
