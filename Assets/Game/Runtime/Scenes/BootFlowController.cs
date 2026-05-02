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
        private Texture2D? _noirBackdropTexture;
        private float _menuInputReadyAtTime;

        private void Start()
        {
            _settings = GameSettingsFileService.LoadOrCreate();
            GameSettingsFileService.Apply(_settings);
            _menuInputReadyAtTime = Time.unscaledTime + 0.65f;

            ResolveSaveGameFileService();
            _saveExists = saveGameFileService != null && saveGameFileService.SaveExists();

            if (ShouldSkipBoot())
            {
                StartNewGame();
            }
        }

        private void Update()
        {
            if (_isLoading)
            {
                return;
            }

            if (!CanAcceptMenuInput())
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
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_saveExists)
                {
                    ContinueGame();
                }
                else
                {
                    StartNewGame();
                }
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
            Debug.Log("BootFlow loading scene: " + targetScene);
            SceneManager.LoadScene(targetScene);
        }

        private void ResolveSaveGameFileService()
        {
            if (saveGameFileService != null)
            {
                return;
            }

            saveGameFileService = GetComponent<SaveGameFileService>();
            if (saveGameFileService != null)
            {
                return;
            }

            saveGameFileService = FindFirstObjectByType<SaveGameFileService>();
            if (saveGameFileService == null)
            {
                Debug.LogWarning("BootFlowController could not find SaveGameFileService; menu will start games without save persistence.");
            }
        }

        private bool CanAcceptMenuInput()
        {
            return Time.unscaledTime >= _menuInputReadyAtTime;
        }

        private static bool ShouldSkipBoot()
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index += 1)
            {
                var arg = args[index];
                if (string.Equals(arg, "-skipBoot", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "--skip-boot", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "-quickStart", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "--quick-start", System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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
            var acceptsInput = CanAcceptMenuInput();
            var mousePosition = Event.current.mousePosition;
            var hovering = acceptsInput && rect.Contains(mousePosition);
            var pressed = hovering && Event.current.type == EventType.MouseDown && Event.current.button == 0;

            GUI.DrawTexture(new Rect(rect.x + 5f, rect.y + 6f, rect.width, rect.height), _clearTexture);
            GUI.DrawTexture(rect, pressed ? _buttonActiveTexture : hovering ? _buttonHoverTexture : _buttonTexture);
            DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), new Color(0.75f, 0.54f, 0.22f, hovering ? 1f : 0.62f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), new Color(0.95f, 0.78f, 0.42f, hovering ? 0.42f : 0.18f));

            GUI.Label(new Rect(rect.x + 18f, rect.y + 11f, rect.width - 86f, rect.height - 12f), label.ToUpperInvariant(), _buttonTextStyle);
            GUI.Label(new Rect(rect.x + rect.width - 70f, rect.y + 11f, 52f, rect.height - 12f), hotkey.ToUpperInvariant(), _hintStyle);

            if (acceptsInput && hovering && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Event.current.Use();
                return true;
            }

            return acceptsInput && GUI.Button(rect, GUIContent.none, GUIStyle.none);
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

        private void DrawBackdrop()
        {
            // Keep the menu in the same noir world as gameplay without adding a
            // screen-space pixel filter over the player's view.
            _noirBackdropTexture ??= CreateNoirBackdropTexture();
            GUI.DrawTexture(
                new Rect(0f, 0f, Screen.width, Screen.height),
                _noirBackdropTexture,
                ScaleMode.StretchToFill,
                false);

            DrawRect(new Rect(0f, 0f, Screen.width, 36f), new Color(0f, 0f, 0f, 0.54f));
            DrawRect(new Rect(0f, Screen.height - 54f, Screen.width, 54f), new Color(0f, 0f, 0f, 0.64f));
            DrawRect(new Rect(0f, 0f, Screen.width * 0.08f, Screen.height), new Color(0f, 0f, 0f, 0.42f));
            DrawRect(new Rect(Screen.width * 0.92f, 0f, Screen.width * 0.08f, Screen.height), new Color(0f, 0f, 0f, 0.48f));
        }

        private static Texture2D CreateNoirBackdropTexture()
        {
            const int width = 384;
            const int height = 216;
            var pixels = new Color[width * height];

            for (var y = 0; y < height; y += 1)
            {
                var t = y / (float)(height - 1);
                for (var x = 0; x < width; x += 1)
                {
                    var noise = Hash01(x, y) - 0.5f;
                    var baseColor = Color.Lerp(
                        new Color(0.010f, 0.014f, 0.018f, 1f),
                        new Color(0.070f, 0.044f, 0.030f, 1f),
                        Mathf.Pow(t, 1.35f));
                    var surfaceVariation = noise * 0.012f;
                    pixels[y * width + x] = new Color(baseColor.r + surfaceVariation, baseColor.g + surfaceVariation, baseColor.b + surfaceVariation, 1f);
                }
            }

            FillRect(pixels, width, height, 182, 0, 78, 216, new Color(0.012f, 0.014f, 0.015f, 0.88f));
            FillRect(pixels, width, height, 248, 0, 2, 216, new Color(0.74f, 0.52f, 0.24f, 0.38f));
            FillRect(pixels, width, height, 144, 171, 240, 45, new Color(0.040f, 0.035f, 0.029f, 0.66f));

            for (var i = 0; i < 15; i += 1)
            {
                var x = 128 + i * 16;
                var buildingWidth = 11 + (i % 4) * 3;
                var buildingHeight = 48 + (i % 5) * 9;
                FillRect(pixels, width, height, x, 38, buildingWidth, buildingHeight, new Color(0.004f, 0.006f, 0.008f, 0.84f));

                for (var window = 0; window < 5; window += 1)
                {
                    if ((window + i) % 3 == 0)
                    {
                        FillRect(pixels, width, height, x + 3, 48 + window * 12, 4, 2, new Color(0.82f, 0.55f, 0.24f, 0.30f));
                    }
                }
            }

            for (var i = 0; i < 10; i += 1)
            {
                FillRect(pixels, width, height, 294, 16 + i * 20, 14, 4, new Color(0.82f, 0.58f, 0.28f, 0.42f));
                FillRect(pixels, width, height, 92, 30 + i * 18, 8, 2, new Color(0.75f, 0.58f, 0.36f, 0.16f));
            }

            FillRect(pixels, width, height, 276, 52, 72, 24, new Color(0.16f, 0.054f, 0.034f, 0.88f));
            FillRect(pixels, width, height, 276, 52, 72, 2, new Color(0.92f, 0.58f, 0.22f, 0.82f));
            for (var i = 0; i < 7; i += 1)
            {
                FillRect(pixels, width, height, 283 + i * 10, 57, 2, 2, new Color(1f, 0.66f, 0.26f, 0.90f));
            }

            FillGlow(pixels, width, height, 306, 82, 42, new Color(0.96f, 0.58f, 0.22f, 0.18f));
            FillGlow(pixels, width, height, 330, 156, 54, new Color(0.96f, 0.70f, 0.36f, 0.20f));
            FillRect(pixels, width, height, 258, 132, 90, 9, new Color(0.004f, 0.005f, 0.006f, 0.97f));
            FillRect(pixels, width, height, 282, 121, 36, 12, new Color(0.004f, 0.005f, 0.006f, 0.97f));
            FillRect(pixels, width, height, 328, 135, 9, 2, new Color(1f, 0.76f, 0.44f, 0.95f));
            FillRect(pixels, width, height, 337, 136, 42, 1, new Color(1f, 0.76f, 0.44f, 0.25f));

            FillGlow(pixels, width, height, 257, 130, 52, new Color(0.90f, 0.42f, 0.14f, 0.10f));
            FillRect(pixels, width, height, 238, 106, 52, 8, new Color(0.005f, 0.004f, 0.004f, 0.99f));
            FillRect(pixels, width, height, 250, 92, 20, 17, new Color(0.005f, 0.004f, 0.004f, 0.98f));
            FillRect(pixels, width, height, 244, 114, 32, 67, new Color(0.006f, 0.005f, 0.004f, 0.99f));
            FillRect(pixels, width, height, 276, 118, 2, 62, new Color(0.78f, 0.43f, 0.16f, 0.22f));
            FillRect(pixels, width, height, 238, 106, 52, 1, new Color(0.85f, 0.52f, 0.20f, 0.38f));
            FillRect(pixels, width, height, 278, 133, 21, 1, new Color(0.95f, 0.67f, 0.32f, 0.50f));
            FillRect(pixels, width, height, 298, 132, 2, 2, new Color(1f, 0.45f, 0.12f, 0.94f));

            for (var i = 0; i < 90; i += 1)
            {
                var x = Mathf.FloorToInt(Hash01(i * 31, i * 17) * width);
                var y = Mathf.FloorToInt(Hash01(i * 47, i * 13) * height);
                var rainLength = 2 + (i % 5);
                FillRect(pixels, width, height, x, y, 1, rainLength, new Color(0.42f, 0.50f, 0.56f, 0.15f));
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "NoirBootPlate",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return texture;
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

        private static void FillRect(Color[] pixels, int textureWidth, int textureHeight, int x, int y, int width, int height, Color color)
        {
            var minX = Mathf.Clamp(x, 0, textureWidth);
            var maxX = Mathf.Clamp(x + width, 0, textureWidth);
            var minY = Mathf.Clamp(y, 0, textureHeight);
            var maxY = Mathf.Clamp(y + height, 0, textureHeight);

            for (var py = minY; py < maxY; py += 1)
            {
                for (var px = minX; px < maxX; px += 1)
                {
                    var index = py * textureWidth + px;
                    pixels[index] = Blend(pixels[index], color);
                }
            }
        }

        private static void FillGlow(Color[] pixels, int textureWidth, int textureHeight, int centerX, int centerY, int radius, Color color)
        {
            var radiusSquared = radius * radius;
            for (var y = Mathf.Max(0, centerY - radius); y < Mathf.Min(textureHeight, centerY + radius); y += 1)
            {
                for (var x = Mathf.Max(0, centerX - radius); x < Mathf.Min(textureWidth, centerX + radius); x += 1)
                {
                    var dx = x - centerX;
                    var dy = (y - centerY) * 1.6f;
                    var distanceSquared = dx * dx + dy * dy;
                    if (distanceSquared > radiusSquared)
                    {
                        continue;
                    }

                    var intensity = 1f - Mathf.Sqrt(distanceSquared / radiusSquared);
                    pixels[y * textureWidth + x] = Blend(pixels[y * textureWidth + x], new Color(color.r, color.g, color.b, color.a * intensity));
                }
            }
        }

        private static Color Blend(Color destination, Color source)
        {
            var alpha = Mathf.Clamp01(source.a);
            return new Color(
                Mathf.Lerp(destination.r, source.r, alpha),
                Mathf.Lerp(destination.g, source.g, alpha),
                Mathf.Lerp(destination.b, source.b, alpha),
                1f);
        }

        private static float Hash01(int x, int y)
        {
            unchecked
            {
                var value = x * 374761393 + y * 668265263;
                value = (value ^ (value >> 13)) * 1274126177;
                return ((value ^ (value >> 16)) & 0x00FFFFFF) / 16777215f;
            }
        }
    }
}
