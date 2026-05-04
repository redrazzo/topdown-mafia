using System.Collections;
using System.IO;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Diagnostics
{
    public sealed class LaunchScreenshotCapture : MonoBehaviour
    {
        private const string CaptureFlag = "-captureScreenshot";
        private const string CaptureFlagLong = "--capture-screenshot";

        [SerializeField] private float captureDelaySeconds = 1.2f;

        private void Start()
        {
            if (TryResolveCapturePath(out var capturePath))
            {
                StartCoroutine(CaptureAndQuit(capturePath));
            }
        }

        private IEnumerator CaptureAndQuit(string capturePath)
        {
            if (captureDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(captureDelaySeconds);
            }

            yield return new WaitForEndOfFrame();

            var directory = Path.GetDirectoryName(capturePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            ScreenCapture.CaptureScreenshot(capturePath);
            Debug.Log("Launch screenshot captured: " + capturePath);
            yield return new WaitForSecondsRealtime(0.35f);
            Application.Quit();
        }

        private static bool TryResolveCapturePath(out string capturePath)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index += 1)
            {
                if (string.Equals(args[index], CaptureFlag, System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(args[index], CaptureFlagLong, System.StringComparison.OrdinalIgnoreCase))
                {
                    capturePath = args[index + 1];
                    return !string.IsNullOrWhiteSpace(capturePath);
                }
            }

            capturePath = string.Empty;
            return false;
        }
    }
}
