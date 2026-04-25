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
    }
}
