using System.IO;
using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public static class GameSettingsFileService
    {
        private const string FileName = "settings.json";
        private static GameSettingsData? _cachedSettings;

        private static string SettingsPath => Path.Combine(Application.persistentDataPath, FileName);

        public static GameSettingsData LoadOrCreate()
        {
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            if (!File.Exists(SettingsPath))
            {
                _cachedSettings = new GameSettingsData();
                Save(_cachedSettings);
                return _cachedSettings;
            }

            var json = File.ReadAllText(SettingsPath);
            _cachedSettings = JsonUtility.FromJson<GameSettingsData>(json) ?? new GameSettingsData();
            Normalize(_cachedSettings);
            return _cachedSettings;
        }

        public static void Save(GameSettingsData settings)
        {
            _cachedSettings = settings;
            var json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(SettingsPath, json);
            Apply(settings);
        }

        public static void ApplyLoaded()
        {
            Apply(LoadOrCreate());
        }

        public static void Apply(GameSettingsData settings)
        {
            Normalize(settings);
            var audioListenerType = System.Type.GetType("UnityEngine.AudioListener, UnityEngine.AudioModule");
            var volumeProperty = audioListenerType == null ? null : audioListenerType.GetProperty("volume");
            volumeProperty?.SetValue(null, Mathf.Clamp01(settings.MasterVolume));

            var mode = settings.Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.SetResolution(settings.ResolutionWidth, settings.ResolutionHeight, mode);
        }

        public static void CycleResolution(GameSettingsData settings)
        {
            var current = settings.ResolutionWidth + "x" + settings.ResolutionHeight;
            if (current == "1280x720")
            {
                settings.ResolutionWidth = 1600;
                settings.ResolutionHeight = 900;
            }
            else if (current == "1600x900")
            {
                settings.ResolutionWidth = 1920;
                settings.ResolutionHeight = 1080;
            }
            else
            {
                settings.ResolutionWidth = 1280;
                settings.ResolutionHeight = 720;
            }
        }

        private static void Normalize(GameSettingsData settings)
        {
            if (settings.ResolutionWidth <= 0)
            {
                settings.ResolutionWidth = 1920;
            }

            if (settings.ResolutionHeight <= 0)
            {
                settings.ResolutionHeight = 1080;
            }

            if (settings.InputSensitivity <= 0f)
            {
                settings.InputSensitivity = 1f;
            }

            settings.ResolutionWidth = Mathf.Max(960, settings.ResolutionWidth);
            settings.ResolutionHeight = Mathf.Max(540, settings.ResolutionHeight);
            settings.InputSensitivity = Mathf.Clamp(settings.InputSensitivity, 0.75f, 1.35f);
            settings.MasterVolume = Mathf.Clamp01(settings.MasterVolume);
            settings.HudScale = Mathf.Clamp(settings.HudScale, 0.85f, 1.35f);
        }
    }
}
