using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public static class HudStyleUtility
    {
        private static GUIStyle? _titleStyle;
        private static GUIStyle? _bodyStyle;
        private static GUIStyle? _promptStyle;
        private static GUIStyle? _menuTitleStyle;
        private static GUIStyle? _menuBodyStyle;
        private static GUIStyle? _buttonStyle;
        private static Texture2D? _panelTexture;
        private static Texture2D? _shadowTexture;
        private static Texture2D? _goldTexture;
        private static Texture2D? _edgeTexture;
        private static Texture2D? _paperTexture;

        public static GUIStyle MenuTitleStyle
        {
            get
            {
                EnsureStyles();
                return _menuTitleStyle!;
            }
        }

        public static GUIStyle MenuBodyStyle
        {
            get
            {
                EnsureStyles();
                return _menuBodyStyle!;
            }
        }

        public static GUIStyle ButtonStyle
        {
            get
            {
                EnsureStyles();
                return _buttonStyle!;
            }
        }

        public static void DrawPanel(Rect rect, string title, string body)
        {
            EnsureStyles();
            var hudScale = GameSettingsFileService.LoadOrCreate().HudScale;
            rect = new Rect(rect.x, rect.y, rect.width * hudScale, rect.height * hudScale);

            DrawFrame(rect, true);

            GUI.Label(new Rect(rect.x + 17f, rect.y + 9f, rect.width - 34f, 20f), title.ToUpperInvariant(), _titleStyle);
            GUI.Label(new Rect(rect.x + 17f, rect.y + 32f, rect.width - 34f, rect.height - 39f), body, _bodyStyle);
        }

        public static void DrawPrompt(Rect rect, string text)
        {
            EnsureStyles();
            GUI.DrawTexture(new Rect(rect.x + 4f, rect.y + 4f, rect.width, rect.height), _shadowTexture);
            GUI.DrawTexture(rect, _panelTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 4f, rect.height), _goldTexture);
            GUI.DrawTexture(new Rect(rect.x + 4f, rect.y, 1f, rect.height), _edgeTexture);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 20f, rect.height - 10f), text, _promptStyle);
        }

        public static void DrawMenuFrame(Rect rect, string title, string subtitle)
        {
            EnsureStyles();
            DrawFrame(rect, false);
            GUI.DrawTexture(new Rect(rect.x + 18f, rect.y + 48f, rect.width - 36f, 1f), _edgeTexture);
            GUI.Label(new Rect(rect.x + 22f, rect.y + 15f, rect.width - 44f, 24f), title.ToUpperInvariant(), _menuTitleStyle);
            GUI.Label(new Rect(rect.x + 22f, rect.y + 54f, rect.width - 44f, 38f), subtitle, _menuBodyStyle);
        }

        public static Rect CenteredRect(float width, float height)
        {
            return new Rect(
                Mathf.Max(16f, (Screen.width - width) * 0.5f),
                Mathf.Max(16f, (Screen.height - height) * 0.5f),
                width,
                height);
        }

        private static void DrawFrame(Rect rect, bool compact)
        {
            GUI.DrawTexture(new Rect(rect.x + 6f, rect.y + 7f, rect.width, rect.height), _shadowTexture);
            GUI.DrawTexture(rect, _panelTexture);
            GUI.DrawTexture(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 10f), _paperTexture);

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 2f, rect.height), _edgeTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 2f, rect.y, 2f, rect.height), _edgeTexture);
            GUI.DrawTexture(new Rect(rect.x + 10f, rect.y + 10f, compact ? 34f : 48f, 2f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.x + 10f, rect.y + 10f, 2f, compact ? 18f : 28f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.xMax - (compact ? 44f : 58f), rect.yMax - 12f, compact ? 34f : 48f, 2f), _goldTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 12f, rect.yMax - (compact ? 30f : 40f), 2f, compact ? 18f : 28f), _goldTexture);
        }

        private static void EnsureStyles()
        {
            if (_panelTexture != null)
            {
                return;
            }

            _panelTexture = MakeSolidTexture(new Color(0.025f, 0.022f, 0.018f, 0.84f));
            _shadowTexture = MakeSolidTexture(new Color(0f, 0f, 0f, 0.46f));
            _goldTexture = MakeSolidTexture(new Color(0.77f, 0.55f, 0.24f, 0.92f));
            _edgeTexture = MakeSolidTexture(new Color(0.12f, 0.095f, 0.062f, 0.92f));
            _paperTexture = MakeSolidTexture(new Color(0.09f, 0.073f, 0.052f, 0.18f));

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.9f, 0.72f, 0.38f, 1f) },
                wordWrap = false
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.88f, 0.84f, 0.72f, 1f) },
                wordWrap = true
            };

            _promptStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.92f, 0.82f, 0.62f, 1f) },
                wordWrap = false
            };

            _menuTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.95f, 0.77f, 0.44f, 1f) },
                wordWrap = false
            };

            _menuBodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.86f, 0.8f, 0.68f, 1f) },
                wordWrap = true
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(16, 12, 7, 7),
                normal =
                {
                    textColor = new Color(0.92f, 0.84f, 0.68f, 1f),
                    background = MakeSolidTexture(new Color(0.1f, 0.085f, 0.065f, 0.84f))
                },
                hover =
                {
                    textColor = new Color(1f, 0.9f, 0.62f, 1f),
                    background = MakeSolidTexture(new Color(0.18f, 0.13f, 0.075f, 0.92f))
                },
                active =
                {
                    textColor = new Color(1f, 0.82f, 0.42f, 1f),
                    background = MakeSolidTexture(new Color(0.22f, 0.15f, 0.075f, 0.95f))
                }
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
