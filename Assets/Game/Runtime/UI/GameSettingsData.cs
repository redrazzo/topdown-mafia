using System;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public bool Fullscreen = true;
        public float MasterVolume = 0.75f;
        public bool Subtitles = true;
        public float HudScale = 1f;
        public int ResolutionWidth = 1920;
        public int ResolutionHeight = 1080;
        public float InputSensitivity = 1f;
    }
}
