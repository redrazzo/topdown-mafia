using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public static class HudStyleUtility
    {
        private static GUIStyle? _panelStyle;
        private static GUIStyle? _titleStyle;
        private static GUIStyle? _bodyStyle;

        public static void DrawPanel(Rect rect, string title, string body)
        {
            EnsureStyles();
            var hudScale = GameSettingsFileService.LoadOrCreate().HudScale;
            rect = new Rect(rect.x, rect.y, rect.width * hudScale, rect.height * hudScale);

            var originalColor = GUI.color;

            GUI.color = new Color(0f, 0f, 0f, 0.22f);
            GUI.Box(new Rect(rect.x + 4f, rect.y + 4f, rect.width, rect.height), GUIContent.none, _panelStyle);

            GUI.color = new Color(0.09f, 0.08f, 0.07f, 0.9f);
            GUI.Box(rect, GUIContent.none, _panelStyle);

            GUI.color = new Color(0.73f, 0.58f, 0.28f, 1f);
            GUI.Box(new Rect(rect.x, rect.y, rect.width, 5f), GUIContent.none, _panelStyle);

            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 14f, rect.y + 10f, rect.width - 28f, 24f), title, _titleStyle);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 36f, rect.width - 28f, rect.height - 44f), body, _bodyStyle);

            GUI.color = originalColor;
        }

        private static void EnsureStyles()
        {
            if (_panelStyle != null)
            {
                return;
            }

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = new Color(0.92f, 0.84f, 0.7f, 1f) },
                wordWrap = false
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.83f, 0.8f, 0.74f, 1f) },
                wordWrap = true
            };
        }
    }
}
