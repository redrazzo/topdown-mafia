using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public static class HudStyleUtility
    {
        private static GUIStyle? _titleStyle;
        private static GUIStyle? _bodyStyle;
        private static GUIStyle? _promptStyle;
        private static Texture2D? _panelTexture;
        private static Texture2D? _shadowTexture;
        private static Texture2D? _goldTexture;

        public static void DrawPanel(Rect rect, string title, string body)
        {
            EnsureStyles();
            var hudScale = GameSettingsFileService.LoadOrCreate().HudScale;
            rect = new Rect(rect.x, rect.y, rect.width * hudScale, rect.height * hudScale);

            GUI.DrawTexture(new Rect(rect.x + 5f, rect.y + 5f, rect.width, rect.height), _shadowTexture);
            GUI.DrawTexture(rect, _panelTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), _goldTexture);

            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 24f, 20f), title.ToUpperInvariant(), _titleStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 30f, rect.width - 24f, rect.height - 36f), body, _bodyStyle);
        }

        public static void DrawPrompt(Rect rect, string text)
        {
            EnsureStyles();
            GUI.DrawTexture(new Rect(rect.x + 4f, rect.y + 4f, rect.width, rect.height), _shadowTexture);
            GUI.DrawTexture(rect, _panelTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 3f, rect.height), _goldTexture);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 20f, rect.height - 10f), text, _promptStyle);
        }

        private static void EnsureStyles()
        {
            if (_panelTexture != null)
            {
                return;
            }

            _panelTexture = MakeSolidTexture(new Color(0.025f, 0.022f, 0.018f, 0.84f));
            _shadowTexture = MakeSolidTexture(new Color(0f, 0f, 0f, 0.38f));
            _goldTexture = MakeSolidTexture(new Color(0.72f, 0.52f, 0.22f, 0.9f));

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.86f, 0.68f, 0.36f, 1f) },
                wordWrap = false
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.84f, 0.8f, 0.7f, 1f) },
                wordWrap = true
            };

            _promptStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.92f, 0.82f, 0.62f, 1f) },
                wordWrap = false
            };
        }

        private static Texture2D MakeSolidTexture(Color color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Point
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
