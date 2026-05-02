using MafiaTopDown.Gameplay.Domain.Progression;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class BootFlowController : MonoBehaviour
    {
        [SerializeField] private string firstPlayableScene = "District_01";
        [SerializeField] private string firstPlayableSpawnPoint = "PickupSpawn";
        [SerializeField] private SaveGameFileService saveGameFileService;
        [SerializeField] private bool loadLastSceneFromSave = true;
        [SerializeField] private Rect menuRect = new Rect(56f, 58f, 540f, 610f);
        [SerializeField] private string title = "Mafia Topdown City";
        [SerializeField] private string subtitle = "A quiet favor starts the fall.";

        private bool _saveExists;
        private bool _isLoading;
        private bool _showSettings;
        private GameSettingsData? _settings;
        private GUIStyle? _titleStyle;
        private GUIStyle? _subtitleStyle;
        private GUIStyle? _bodyStyle;
        private GUIStyle? _smallCapsStyle;
        private GUIStyle? _buttonTextStyle;
        private GUIStyle? _hintStyle;
        private Texture2D? _clearTexture;
        private Texture2D? _panelTexture;
        private Texture2D? _buttonTexture;
        private Texture2D? _buttonHoverTexture;
        private Texture2D? _buttonActiveTexture;
        private Texture2D? _goldTexture;

        private void Start()
        {
            _settings = GameSettingsFileService.LoadOrCreate();
            GameSettingsFileService.Apply(_settings);

            if (saveGameFileService == null)
            {
                BeginLoad(firstPlayableScene);
                return;
            }

            _saveExists = saveGameFileService.SaveExists();
        }

        private void Update()
        {
            if (_isLoading)
            {
                return;
            }

            if (_showSettings)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _showSettings = false;
                }

                return;
            }

            if (_saveExists && Input.GetKeyDown(KeyCode.C))
            {
                ContinueGame();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                StartNewGame();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _showSettings = true;
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();
            DrawPosterPanel(menuRect);

            if (_isLoading)
            {
                DrawLoading(menuRect);
                return;
            }

            if (_showSettings)
            {
                DrawSettings(menuRect);
            }
            else
            {
                DrawMainMenu(menuRect);
            }
        }

        private void ContinueGame()
        {
            if (!_saveExists || saveGameFileService == null)
            {
                StartNewGame();
                return;
            }

            var targetScene = firstPlayableScene;
            var targetSpawnPoint = firstPlayableSpawnPoint;
            if (loadLastSceneFromSave)
            {
                var save = saveGameFileService.LoadOrCreateSave();
                var resumeLocation = ResumeLocationResolver.Resolve(save, firstPlayableScene, firstPlayableSpawnPoint);
                targetScene = resumeLocation.SceneName;
                targetSpawnPoint = resumeLocation.SpawnPointId;

                if (save.LastSceneName != targetScene || save.LastSpawnPointId != targetSpawnPoint)
                {
                    saveGameFileService.UpdateSceneLocation(targetScene, targetSpawnPoint);
                }
            }

            BeginLoad(targetScene);
        }

        private void StartNewGame()
        {
            if (saveGameFileService != null)
            {
                saveGameFileService.CreateFreshSave();
            }

            _saveExists = true;
            BeginLoad(firstPlayableScene);
        }

        private void BeginLoad(string targetScene)
        {
            if (_isLoading || string.IsNullOrWhiteSpace(targetScene))
            {
                return;
            }

            _isLoading = true;
            SceneManager.LoadScene(targetScene);
        }

        private void DrawMainMenu(Rect panel)
        {
            var x = panel.x + 36f;
            var y = panel.y + 34f;
            var width = panel.width - 72f;

            GUI.Label(new Rect(x, y, width, 22f), "PORT CITY, 1933", _smallCapsStyle);
            y += 34f;
            GUI.Label(new Rect(x, y, width, 62f), title.ToUpperInvariant(), _titleStyle);
            y += 72f;
            GUI.Label(new Rect(x, y, width, 42f), subtitle, _subtitleStyle);
            y += 60f;

            DrawRule(new Rect(x, y, width, 2f));
            y += 24f;
            GUI.Label(
                new Rect(x, y, width, 88f),
                _saveExists
                    ? "A rain-slick harbor, a borrowed sedan, and a favor that keeps getting heavier. Continue the last job or burn a clean save from the docks."
                    : "A rain-slick harbor, a borrowed sedan, and a favor that keeps getting heavier. Start from the docks and keep your head down.",
                _bodyStyle);
            y += 110f;

            if (_saveExists && DrawMenuButton(new Rect(x, y, width, 46f), "Continue Last Job", "C"))
            {
                ContinueGame();
            }

            y += 56f;
            if (DrawMenuButton(new Rect(x, y, width, 46f), "New Game", "N"))
            {
                StartNewGame();
            }

            y += 56f;
            if (DrawMenuButton(new Rect(x, y, width, 46f), "Settings", "S"))
            {
                _showSettings = true;
            }

            y += 74f;
            DrawRule(new Rect(x, y, width, 1f));
            y += 16f;
            GUI.Label(new Rect(x, y, width, 44f), "WASD DRIVE / WALK     E INTERACT     SPACE FIRE     ESC PAUSE", _smallCapsStyle);
        }

        private void DrawLoading(Rect panel)
        {
            var x = panel.x + 36f;
            var y = panel.y + 44f;
            var width = panel.width - 72f;
            GUI.Label(new Rect(x, y, width, 60f), "LOADING THE HARBOR", _titleStyle);
            y += 82f;
            DrawRule(new Rect(x, y, width, 2f));
            y += 24f;
            GUI.Label(new Rect(x, y, width, 80f), "Engines low. Rain on the windshield. Someone is waiting in the back office.", _bodyStyle);
        }

        private void DrawSettings(Rect panel)
        {
            if (_settings == null)
            {
                return;
            }

            var x = panel.x + 36f;
            var y = panel.y + 34f;
            var width = panel.width - 72f;

            GUI.Label(new Rect(x, y, width, 22f), "OPTIONS", _smallCapsStyle);
            y += 40f;
            GUI.Label(new Rect(x, y, width, 52f), "SETTINGS", _titleStyle);
            y += 68f;
            GUI.Label(new Rect(x, y, width, 48f), "Keep the presentation readable without dragging the city out of its smoke.", _bodyStyle);
            y += 68f;

            var previousVolume = _settings.MasterVolume;
            var previousHudScale = _settings.HudScale;
            var previousFullscreen = _settings.Fullscreen;
            var previousSubtitles = _settings.Subtitles;

            DrawSettingLabel(new Rect(x, y, width, 24f), "Master Volume", Mathf.RoundToInt(_settings.MasterVolume * 100f) + "%");
            y += 28f;
            _settings.MasterVolume = GUI.HorizontalSlider(new Rect(x, y, width, 22f), _settings.MasterVolume, 0f, 1f);
            y += 52f;

            _settings.Fullscreen = DrawToggle(new Rect(x, y, width, 42f), "Fullscreen", _settings.Fullscreen);
            y += 52f;
            _settings.Subtitles = DrawToggle(new Rect(x, y, width, 42f), "Subtitles", _settings.Subtitles);
            y += 60f;

            DrawSettingLabel(new Rect(x, y, width, 24f), "HUD Scale", _settings.HudScale.ToString("0.00") + "x");
            y += 28f;
            _settings.HudScale = GUI.HorizontalSlider(new Rect(x, y, width, 22f), _settings.HudScale, 0.85f, 1.35f);
            y += 64f;

            if (!Mathf.Approximately(previousVolume, _settings.MasterVolume) ||
                !Mathf.Approximately(previousHudScale, _settings.HudScale) ||
                previousFullscreen != _settings.Fullscreen ||
                previousSubtitles != _settings.Subtitles)
            {
                GameSettingsFileService.Save(_settings);
                GameSettingsFileService.Apply(_settings);
            }

            if (DrawMenuButton(new Rect(x, y, width, 46f), "Back", "ESC"))
            {
                _showSettings = false;
            }
        }

        private void DrawSettingLabel(Rect rect, string label, string value)
        {
            GUI.Label(rect, label.ToUpperInvariant(), _smallCapsStyle);
            GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), value, _hintStyle);
        }

        private bool DrawToggle(Rect rect, string label, bool value)
        {
            var toggled = DrawMenuButton(rect, label, value ? "ON" : "OFF");
            return toggled ? !value : value;
        }

        private void DrawPosterPanel(Rect rect)
        {
            DrawRect(new Rect(rect.x + 10f, rect.y + 12f, rect.width, rect.height), new Color(0f, 0f, 0f, 0.42f));
            GUI.DrawTexture(rect, _panelTexture);
            DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), new Color(0.74f, 0.53f, 0.23f, 0.95f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 2f), new Color(0.74f, 0.53f, 0.23f, 0.82f));
            DrawRect(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), new Color(0.74f, 0.53f, 0.23f, 0.46f));
        }

        private bool DrawMenuButton(Rect rect, string label, string hotkey)
        {
            var mousePosition = Event.current.mousePosition;
            var hovering = rect.Contains(mousePosition);
            var pressed = hovering && Event.current.type == EventType.MouseDown && Event.current.button == 0;

            GUI.DrawTexture(new Rect(rect.x + 5f, rect.y + 6f, rect.width, rect.height), _clearTexture);
            GUI.DrawTexture(rect, pressed ? _buttonActiveTexture : hovering ? _buttonHoverTexture : _buttonTexture);
            DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), new Color(0.75f, 0.54f, 0.22f, hovering ? 1f : 0.62f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), new Color(0.95f, 0.78f, 0.42f, hovering ? 0.42f : 0.18f));

            GUI.Label(new Rect(rect.x + 18f, rect.y + 11f, rect.width - 86f, rect.height - 12f), label.ToUpperInvariant(), _buttonTextStyle);
            GUI.Label(new Rect(rect.x + rect.width - 70f, rect.y + 11f, 52f, rect.height - 12f), hotkey.ToUpperInvariant(), _hintStyle);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DrawRule(Rect rect)
        {
            GUI.DrawTexture(rect, _goldTexture);
        }

        private void EnsureStyles()
        {
            if (_panelTexture != null)
            {
                return;
            }

            _clearTexture = MakeTexture(new Color(0f, 0f, 0f, 0.24f));
            _panelTexture = MakeTexture(new Color(0.018f, 0.017f, 0.015f, 0.86f));
            _buttonTexture = MakeTexture(new Color(0.055f, 0.052f, 0.046f, 0.92f));
            _buttonHoverTexture = MakeTexture(new Color(0.11f, 0.082f, 0.052f, 0.96f));
            _buttonActiveTexture = MakeTexture(new Color(0.2f, 0.13f, 0.07f, 0.98f));
            _goldTexture = MakeTexture(new Color(0.78f, 0.57f, 0.27f, 0.92f));

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.94f, 0.82f, 0.57f, 1f) },
                wordWrap = true
            };

            _subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.72f, 0.55f, 0.31f, 1f) },
                wordWrap = true
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.83f, 0.79f, 0.67f, 1f) },
                wordWrap = true
            };

            _smallCapsStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.64f, 0.49f, 0.28f, 1f) },
                wordWrap = true
            };

            _buttonTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.91f, 0.82f, 0.64f, 1f) },
                wordWrap = false
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.78f, 0.6f, 0.34f, 1f) },
                wordWrap = false
            };
        }

        private static void DrawBackdrop()
        {
            var width = Screen.width;
            var height = Screen.height;

            for (var band = 0; band < 18; band += 1)
            {
                var t = band / 17f;
                var color = Color.Lerp(new Color(0.018f, 0.024f, 0.03f, 1f), new Color(0.075f, 0.053f, 0.041f, 1f), t);
                DrawRect(new Rect(0f, height * t, width, height / 17f + 2f), color);
            }

            DrawSkyline(width, height);
            DrawWetStreet(width, height);
            DrawMarquee(width, height);
            DrawStreetLamp(width, height);
            DrawCarSilhouette(width, height);
            DrawFedoraSilhouette(width, height);
            DrawRain(width, height);
            DrawVignette(width, height);
        }

        private static void DrawSkyline(float width, float height)
        {
            var baseY = height * 0.18f;
            for (var index = 0; index < 16; index += 1)
            {
                var x = width * 0.34f + index * width * 0.045f;
                var buildingWidth = width * (0.032f + (index % 3) * 0.009f);
                var buildingHeight = height * (0.14f + (index % 5) * 0.025f);
                DrawRect(new Rect(x, baseY - buildingHeight, buildingWidth, height), new Color(0.01f, 0.013f, 0.017f, 0.82f));

                for (var window = 0; window < 5; window += 1)
                {
                    if ((window + index) % 3 == 0)
                    {
                        DrawRect(
                            new Rect(x + buildingWidth * 0.22f, baseY - buildingHeight + 18f + window * 30f, buildingWidth * 0.18f, 7f),
                            new Color(0.87f, 0.62f, 0.28f, 0.34f));
                    }
                }
            }
        }

        private static void DrawWetStreet(float width, float height)
        {
            DrawRect(new Rect(width * 0.58f, 0f, width * 0.18f, height), new Color(0.02f, 0.022f, 0.023f, 0.92f));
            DrawRect(new Rect(width * 0.66f, 0f, 4f, height), new Color(0.76f, 0.56f, 0.26f, 0.46f));
            DrawRect(new Rect(width * 0.585f, 0f, 2f, height), new Color(0.72f, 0.57f, 0.32f, 0.16f));
            DrawRect(new Rect(width * 0.755f, 0f, 2f, height), new Color(0.72f, 0.57f, 0.32f, 0.14f));

            for (var index = 0; index < 11; index += 1)
            {
                var y = height * 0.07f + index * height * 0.082f;
                DrawRect(new Rect(width * 0.78f, y, 58f, 14f), new Color(0.91f, 0.7f, 0.36f, 0.35f));
                DrawRect(new Rect(width * 0.46f, y + 24f, 38f, 8f), new Color(0.75f, 0.63f, 0.45f, 0.13f));
            }

            DrawRect(new Rect(0f, height * 0.79f, width, height * 0.22f), new Color(0.045f, 0.04f, 0.034f, 0.72f));
            for (var index = 0; index < 8; index += 1)
            {
                var x = width * 0.35f + index * width * 0.075f;
                DrawRect(new Rect(x, height * 0.82f + (index % 2) * 18f, width * 0.05f, 4f), new Color(0.85f, 0.68f, 0.38f, 0.16f));
            }
        }

        private static void DrawMarquee(float width, float height)
        {
            var sign = new Rect(width * 0.69f, height * 0.18f, width * 0.22f, 74f);
            DrawRect(new Rect(sign.x + 8f, sign.y + 10f, sign.width, sign.height), new Color(0f, 0f, 0f, 0.34f));
            DrawRect(sign, new Color(0.18f, 0.075f, 0.052f, 0.88f));
            DrawRect(new Rect(sign.x, sign.y, sign.width, 4f), new Color(0.9f, 0.62f, 0.24f, 0.82f));
            DrawRect(new Rect(sign.x + 18f, sign.y + 26f, sign.width - 36f, 3f), new Color(1f, 0.72f, 0.28f, 0.55f));
            DrawRect(new Rect(sign.x + 18f, sign.y + 45f, sign.width - 72f, 3f), new Color(1f, 0.72f, 0.28f, 0.3f));

            for (var bulb = 0; bulb < 9; bulb += 1)
            {
                DrawRect(new Rect(sign.x + 18f + bulb * 34f, sign.y + 10f, 7f, 7f), new Color(1f, 0.72f, 0.32f, 0.92f));
            }

            DrawGlow(new Vector2(sign.x + sign.width * 0.58f, sign.y + sign.height + 24f), 92f, new Color(0.94f, 0.58f, 0.22f, 0.22f));
        }

        private static void DrawCarSilhouette(float width, float height)
        {
            var x = width * 0.665f;
            var y = height * 0.70f;
            DrawGlow(new Vector2(x + 220f, y - 4f), 220f, new Color(0.96f, 0.76f, 0.42f, 0.22f));
            DrawRect(new Rect(x - 40f, y + 39f, 390f, 4f), new Color(0.94f, 0.72f, 0.38f, 0.14f));
            DrawRect(new Rect(x, y, 330f, 38f), new Color(0.006f, 0.007f, 0.008f, 0.96f));
            DrawRect(new Rect(x + 72f, y - 42f, 142f, 48f), new Color(0.008f, 0.009f, 0.01f, 0.96f));
            DrawRect(new Rect(x + 24f, y + 27f, 58f, 24f), new Color(0f, 0f, 0f, 0.9f));
            DrawRect(new Rect(x + 238f, y + 27f, 58f, 24f), new Color(0f, 0f, 0f, 0.9f));
            DrawRect(new Rect(x + 272f, y + 10f, 32f, 9f), new Color(1f, 0.8f, 0.48f, 0.98f));
            DrawRect(new Rect(x + 302f, y + 12f, 118f, 4f), new Color(1f, 0.8f, 0.48f, 0.24f));
            DrawRect(new Rect(x - 116f, y + 12f, 116f, 5f), new Color(1f, 0.78f, 0.45f, 0.2f));
        }

        private static void DrawStreetLamp(float width, float height)
        {
            var poleX = width * 0.82f;
            var poleY = height * 0.22f;
            DrawGlow(new Vector2(poleX - 26f, poleY + 12f), 155f, new Color(1f, 0.68f, 0.28f, 0.18f));
            DrawRect(new Rect(poleX, poleY, 5f, height * 0.64f), new Color(0.025f, 0.019f, 0.014f, 0.92f));
            DrawRect(new Rect(poleX - 78f, poleY, 82f, 5f), new Color(0.025f, 0.019f, 0.014f, 0.92f));
            DrawRect(new Rect(poleX - 92f, poleY + 3f, 28f, 12f), new Color(0.9f, 0.55f, 0.22f, 0.84f));
            DrawRect(new Rect(poleX - 136f, poleY + height * 0.62f, 260f, 5f), new Color(0.94f, 0.72f, 0.38f, 0.17f));
        }

        private static void DrawFedoraSilhouette(float width, float height)
        {
            var x = width * 0.66f;
            var y = height * 0.38f;
            DrawGlow(new Vector2(x + 36f, y + 178f), 175f, new Color(0.85f, 0.48f, 0.18f, 0.1f));
            DrawRect(new Rect(x - 92f, y + 336f, 248f, 6f), new Color(0f, 0f, 0f, 0.56f));
            DrawRect(new Rect(x - 34f, y + 12f, 70f, 64f), new Color(0.014f, 0.011f, 0.009f, 0.98f));
            DrawRect(new Rect(x + 34f, y + 16f, 3f, 58f), new Color(0.82f, 0.52f, 0.23f, 0.28f));
            DrawRect(new Rect(x - 98f, y + 72f, 196f, 34f), new Color(0.011f, 0.009f, 0.008f, 0.99f));
            DrawRect(new Rect(x - 98f, y + 72f, 196f, 3f), new Color(0.82f, 0.52f, 0.23f, 0.34f));
            DrawRect(new Rect(x - 58f, y + 98f, 116f, 248f), new Color(0.009f, 0.008f, 0.007f, 0.99f));
            DrawRect(new Rect(x + 54f, y + 102f, 5f, 236f), new Color(0.76f, 0.45f, 0.18f, 0.2f));
            DrawRect(new Rect(x - 104f, y + 124f, 56f, 204f), new Color(0.007f, 0.006f, 0.005f, 0.97f));
            DrawRect(new Rect(x + 48f, y + 124f, 56f, 204f), new Color(0.007f, 0.006f, 0.005f, 0.97f));
            DrawRect(new Rect(x - 20f, y + 124f, 40f, 206f), new Color(0.1f, 0.058f, 0.028f, 0.42f));
            DrawRect(new Rect(x - 58f, y + 118f, 116f, 4f), new Color(0.87f, 0.58f, 0.25f, 0.2f));
            DrawRect(new Rect(x + 62f, y + 184f, 62f, 4f), new Color(0.95f, 0.72f, 0.34f, 0.46f));
            DrawRect(new Rect(x + 120f, y + 180f, 6f, 6f), new Color(1f, 0.55f, 0.18f, 0.92f));
            DrawRect(new Rect(x + 126f, y + 180f, 36f, 2f), new Color(1f, 0.55f, 0.18f, 0.16f));
        }

        private static void DrawRain(float width, float height)
        {
            for (var index = 0; index < 80; index += 1)
            {
                var x = Mathf.Repeat(index * 97f, width);
                var y = Mathf.Repeat(index * 53f, height);
                var length = 12f + (index % 5) * 5f;
                DrawRect(new Rect(x, y, 1f, length), new Color(0.62f, 0.7f, 0.76f, 0.08f));
            }
        }

        private static void DrawVignette(float width, float height)
        {
            DrawRect(new Rect(0f, 0f, width, 34f), new Color(0f, 0f, 0f, 0.62f));
            DrawRect(new Rect(0f, height - 48f, width, 48f), new Color(0f, 0f, 0f, 0.68f));
            DrawRect(new Rect(0f, 0f, width * 0.11f, height), new Color(0f, 0f, 0f, 0.46f));
            DrawRect(new Rect(width * 0.89f, 0f, width * 0.11f, height), new Color(0f, 0f, 0f, 0.52f));
        }

        private static void DrawGlow(Vector2 center, float radius, Color color)
        {
            for (var layer = 5; layer >= 1; layer -= 1)
            {
                var t = layer / 5f;
                var size = radius * t;
                DrawRect(new Rect(center.x - size, center.y - size * 0.35f, size * 2f, size * 0.7f), new Color(color.r, color.g, color.b, color.a * (1f - t * 0.55f)));
            }
        }

        private static void DrawRect(Rect rect, Color color)
        {
            var originalColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = originalColor;
        }

        private static Texture2D MakeTexture(Color color)
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
