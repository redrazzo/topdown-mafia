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
            var audioListenerType = System.Type.GetType("UnityEngine.AudioListener, UnityEngine.AudioModule");
            var volumeProperty = audioListenerType == null ? null : audioListenerType.GetProperty("volume");
            volumeProperty?.SetValue(null, Mathf.Clamp01(settings.MasterVolume));
            Screen.fullScreenMode = settings.Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }
    }
}
