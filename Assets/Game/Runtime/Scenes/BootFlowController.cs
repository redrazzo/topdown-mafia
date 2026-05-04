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
        [SerializeField] private SaveGameFileService? saveGameFileService;
        [SerializeField] private bool loadLastSceneFromSave = true;
        [SerializeField] private Rect menuRect = new Rect(58f, 72f, 520f, 590f);
        [SerializeField] private string title = "Blood On The Docks";
        [SerializeField] private string subtitle = "A borrowed sedan. A quiet favor. A city that keeps score.";
        [SerializeField] private int defaultSlotIndex = 1;

        private bool _saveExists;
        private bool _isLoading;
        private bool _showSettings;
        private int _selectedSlotIndex = 1;
        private SaveSlotDescriptor[] _slotDescriptors = new SaveSlotDescriptor[0];
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
        private Texture2D? _backdropTexture;
        private float _menuInputReadyAtTime;

        private void Start()
        {
            _settings = GameSettingsFileService.LoadOrCreate();
            GameSettingsFileService.Apply(_settings);
            _menuInputReadyAtTime = Time.unscaledTime + 0.65f;
            ResolveSaveGameFileService();
            _selectedSlotIndex = saveGameFileService == null ? Mathf.Max(1, defaultSlotIndex) : saveGameFileService.ActiveSlotIndex;
            RefreshSlotState();

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

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectSlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SelectSlot(3);
            }
            else if (_saveExists && Input.GetKeyDown(KeyCode.C))
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

        private void SelectSlot(int slotIndex)
        {
            _selectedSlotIndex = slotIndex;
            RefreshSlotState();
        }

        private void RefreshSlotState()
        {
            if (saveGameFileService == null)
            {
                _slotDescriptors = new[] { SaveSlotDescriptor.Empty(_selectedSlotIndex) };
                _saveExists = false;
                return;
            }

            saveGameFileService.SetActiveSlot(_selectedSlotIndex);
            _selectedSlotIndex = saveGameFileService.ActiveSlotIndex;
            _slotDescriptors = saveGameFileService.GetSlotDescriptors();
            var selectedSlot = GetSelectedSlot();
            _saveExists = selectedSlot != null && selectedSlot.Exists;
        }

        private SaveSlotDescriptor? GetSelectedSlot()
        {
            for (var index = 0; index < _slotDescriptors.Length; index += 1)
            {
                if (_slotDescriptors[index].SlotIndex == _selectedSlotIndex)
                {
                    return _slotDescriptors[index];
                }
            }

            return null;
        }

        private void OnGUI()
        {
            EnsureStyles();
            var resolvedMenuRect = ResolveMenuRect();
            DrawBackdrop();
            DrawWorldCaption(resolvedMenuRect);
            DrawPosterPanel(resolvedMenuRect);

            if (_isLoading)
            {
                DrawLoading(resolvedMenuRect);
                return;
            }

            if (_showSettings)
            {
                DrawSettings(resolvedMenuRect);
            }
            else
            {
                DrawMainMenu(resolvedMenuRect);
            }
        }

        private void ContinueGame()
        {
            if (!_saveExists || saveGameFileService == null)
            {
                StartNewGame();
                return;
            }

            saveGameFileService.SetActiveSlot(_selectedSlotIndex);
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
                saveGameFileService.SetActiveSlot(_selectedSlotIndex);
                saveGameFileService.CreateFreshSave();
            }

            RefreshSlotState();
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
            var x = panel.x + 34f;
            var y = panel.y + 30f;
            var width = panel.width - 68f;

            GUI.Label(new Rect(x, y, width, 18f), "PORT CITY, 1933 / OPEN CASE FILE", _smallCapsStyle);
            y += 27f;
            GUI.Label(new Rect(x, y, width, 78f), title.ToUpperInvariant(), _titleStyle);
            y += 80f;
            GUI.Label(new Rect(x, y, width, 46f), subtitle, _subtitleStyle);
            y += 58f;

            DrawRule(new Rect(x, y, width, 2f));
            y += 22f;
            GUI.Label(
                new Rect(x, y, width, 52f),
                "Start in the harbor district with the first playable case. The docks are wet, watched, and boxed in by warehouses that do not forget a debt.",
                _bodyStyle);
            y += 62f;

            GUI.Label(new Rect(x, y, width, 20f), "SAVE SLOTS", _smallCapsStyle);
            y += 24f;
            for (var index = 0; index < _slotDescriptors.Length; index += 1)
            {
                if (DrawSaveSlotRow(new Rect(x, y, width, 48f), _slotDescriptors[index]))
                {
                    SelectSlot(_slotDescriptors[index].SlotIndex);
                }

                y += 54f;
            }

            var selectedSlot = GetSelectedSlot();
            var selectedExists = selectedSlot != null && selectedSlot.Exists;
            y += 2f;
            if (DrawMenuButton(new Rect(x, y, width, 44f), selectedExists ? "Continue Selected Slot" : "Start Selected Slot", selectedExists ? "C" : "ENTER"))
            {
                if (selectedExists)
                {
                    ContinueGame();
                }
                else
                {
                    StartNewGame();
                }
            }

            y += 52f;
            if (DrawMenuButton(new Rect(x, y, width, 44f), "New Game In Selected Slot", "N"))
            {
                StartNewGame();
            }

            y += 52f;
            if (DrawMenuButton(new Rect(x, y, width, 44f), "Settings", "S"))
            {
                _showSettings = true;
            }

            y += 58f;
            DrawRule(new Rect(x, y, width, 1f));
            y += 16f;
            GUI.Label(new Rect(x, y, width, 30f), "1-3 SLOT     ENTER START     WASD MOVE / DRIVE     E USE     ESC PAUSE", _smallCapsStyle);
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
            y += 34f;
            GUI.Label(new Rect(x, y, width, 48f), "SETTINGS", _titleStyle);
            y += 58f;
            GUI.Label(new Rect(x, y, width, 38f), "Keep the presentation readable without dragging the city out of its smoke.", _bodyStyle);
            y += 50f;

            var previousVolume = _settings.MasterVolume;
            var previousHudScale = _settings.HudScale;
            var previousFullscreen = _settings.Fullscreen;
            var previousSubtitles = _settings.Subtitles;
            var previousResolutionWidth = _settings.ResolutionWidth;
            var previousResolutionHeight = _settings.ResolutionHeight;
            var previousInputSensitivity = _settings.InputSensitivity;

            DrawSettingLabel(new Rect(x, y, width, 24f), "Master Volume", Mathf.RoundToInt(_settings.MasterVolume * 100f) + "%");
            y += 28f;
            _settings.MasterVolume = GUI.HorizontalSlider(new Rect(x, y, width, 22f), _settings.MasterVolume, 0f, 1f);
            y += 40f;

            var halfWidth = (width - 12f) * 0.5f;
            _settings.Fullscreen = DrawToggle(new Rect(x, y, halfWidth, 38f), "Fullscreen", _settings.Fullscreen);
            _settings.Subtitles = DrawToggle(new Rect(x + halfWidth + 12f, y, halfWidth, 38f), "Subtitles", _settings.Subtitles);
            y += 48f;

            if (DrawMenuButton(new Rect(x, y, width, 38f), "Resolution " + _settings.ResolutionWidth + " x " + _settings.ResolutionHeight, "R"))
            {
                GameSettingsFileService.CycleResolution(_settings);
            }

            y += 48f;

            DrawSettingLabel(new Rect(x, y, width, 24f), "HUD Scale", _settings.HudScale.ToString("0.00") + "x");
            y += 28f;
            _settings.HudScale = GUI.HorizontalSlider(new Rect(x, y, width, 22f), _settings.HudScale, 0.85f, 1.35f);
            y += 40f;

            DrawSettingLabel(new Rect(x, y, width, 24f), "Input Sensitivity", _settings.InputSensitivity.ToString("0.00") + "x");
            y += 28f;
            _settings.InputSensitivity = GUI.HorizontalSlider(new Rect(x, y, width, 22f), _settings.InputSensitivity, 0.75f, 1.35f);
            y += 42f;

            if (!Mathf.Approximately(previousVolume, _settings.MasterVolume) ||
                !Mathf.Approximately(previousHudScale, _settings.HudScale) ||
                previousFullscreen != _settings.Fullscreen ||
                previousSubtitles != _settings.Subtitles ||
                previousResolutionWidth != _settings.ResolutionWidth ||
                previousResolutionHeight != _settings.ResolutionHeight ||
                !Mathf.Approximately(previousInputSensitivity, _settings.InputSensitivity))
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

        private bool DrawSaveSlotRow(Rect rect, SaveSlotDescriptor slot)
        {
            var acceptsInput = CanAcceptMenuInput();
            var selected = slot.SlotIndex == _selectedSlotIndex;
            var mousePosition = Event.current.mousePosition;
            var hovering = acceptsInput && rect.Contains(mousePosition);
            var pressed = hovering && Event.current.type == EventType.MouseDown && Event.current.button == 0;

            GUI.DrawTexture(new Rect(rect.x + 4f, rect.y + 5f, rect.width, rect.height), _clearTexture);
            GUI.DrawTexture(rect, pressed ? _buttonActiveTexture : selected || hovering ? _buttonHoverTexture : _buttonTexture);
            DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), new Color(0.75f, 0.54f, 0.22f, selected ? 1f : 0.35f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), new Color(0.95f, 0.78f, 0.42f, selected ? 0.42f : 0.12f));

            GUI.Label(new Rect(rect.x + 16f, rect.y + 7f, rect.width - 98f, 19f), "SLOT " + slot.SlotIndex + " / " + slot.DisplayName.ToUpperInvariant(), _buttonTextStyle);
            GUI.Label(new Rect(rect.x + 16f, rect.y + 27f, rect.width - 98f, 18f), slot.Summary, _smallCapsStyle);
            GUI.Label(new Rect(rect.x + rect.width - 66f, rect.y + 13f, 48f, 20f), slot.SlotIndex.ToString(), _hintStyle);

            if (acceptsInput && hovering && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Event.current.Use();
                return true;
            }

            return acceptsInput && GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DrawPosterPanel(Rect rect)
        {
            DrawRect(new Rect(rect.x + 14f, rect.y + 16f, rect.width, rect.height), new Color(0f, 0f, 0f, 0.46f));
            GUI.DrawTexture(rect, _panelTexture);
            DrawRect(new Rect(rect.x, rect.y, 4f, rect.height), new Color(0.72f, 0.50f, 0.20f, 0.95f));
            DrawRect(new Rect(rect.x + 10f, rect.y + 10f, rect.width - 20f, 1f), new Color(0.95f, 0.75f, 0.36f, 0.52f));
            DrawRect(new Rect(rect.x + 10f, rect.yMax - 12f, rect.width - 20f, 1f), new Color(0.95f, 0.75f, 0.36f, 0.30f));
            DrawRect(new Rect(rect.xMax - 1f, rect.y + 10f, 1f, rect.height - 22f), new Color(0.72f, 0.50f, 0.20f, 0.32f));
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

            _clearTexture = MakeTexture(new Color(0f, 0f, 0f, 0.26f));
            _panelTexture = MakeTexture(new Color(0.014f, 0.013f, 0.011f, 0.90f));
            _buttonTexture = MakeTexture(new Color(0.050f, 0.045f, 0.036f, 0.93f));
            _buttonHoverTexture = MakeTexture(new Color(0.125f, 0.085f, 0.045f, 0.97f));
            _buttonActiveTexture = MakeTexture(new Color(0.22f, 0.13f, 0.055f, 0.98f));
            _goldTexture = MakeTexture(new Color(0.82f, 0.58f, 0.24f, 0.92f));
            _backdropTexture = CreateNoirBackdropTexture();

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.98f, 0.84f, 0.55f, 1f) },
                wordWrap = true
            };

            _subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.78f, 0.58f, 0.31f, 1f) },
                wordWrap = true
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.86f, 0.81f, 0.69f, 1f) },
                wordWrap = true
            };

            _smallCapsStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.76f, 0.56f, 0.28f, 1f) },
                wordWrap = true
            };

            _buttonTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.92f, 0.83f, 0.65f, 1f) },
                wordWrap = false
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.78f, 0.6f, 0.34f, 1f) },
                wordWrap = false
            };
        }

        private void DrawBackdrop()
        {
            if (_backdropTexture != null)
            {
                var originalColor = GUI.color;
                GUI.color = new Color(0.9f, 0.82f, 0.68f, 1f);
                GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _backdropTexture, ScaleMode.ScaleAndCrop);
                GUI.color = originalColor;
            }

            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.20f));
            DrawSodiumGlow(new Rect(Screen.width * 0.60f, Screen.height * 0.18f, Screen.width * 0.32f, Screen.height * 0.46f));
            DrawHorizontalGradient(0f, Screen.height, Screen.width * 0.68f, true, new Color(0f, 0f, 0f, 0.91f));
            DrawHorizontalGradient(Screen.width * 0.58f, Screen.height, Screen.width * 0.42f, false, new Color(0f, 0f, 0f, 0.58f));
            DrawVerticalGradient(0f, Screen.width, 170f, true, new Color(0f, 0f, 0f, 0.76f));
            DrawVerticalGradient(Screen.height - 240f, Screen.width, 240f, false, new Color(0f, 0f, 0f, 0.86f));
            DrawScreenRain();
            DrawRect(new Rect(0f, 0f, Screen.width, 24f), new Color(0f, 0f, 0f, 0.88f));
            DrawRect(new Rect(0f, Screen.height - 28f, Screen.width, 28f), new Color(0f, 0f, 0f, 0.88f));
            DrawRect(new Rect(0f, 0f, Screen.width, 1f), new Color(0.85f, 0.61f, 0.28f, 0.25f));
            DrawRect(new Rect(0f, Screen.height - 1f, Screen.width, 1f), new Color(0.85f, 0.61f, 0.28f, 0.20f));
        }

        private Rect ResolveMenuRect()
        {
            var width = Mathf.Clamp(Screen.width * 0.29f, 492f, 560f);
            var height = Mathf.Clamp(Screen.height * 0.61f, 570f, 640f);
            var x = Mathf.Clamp(Screen.width * 0.035f, 42f, 76f);
            var y = Mathf.Max(42f, (Screen.height - height) * 0.5f);
            return new Rect(x, y, width, height);
        }

        private void DrawWorldCaption(Rect panel)
        {
            if (Screen.width < 1100)
            {
                return;
            }

            var width = Mathf.Clamp(Screen.width * 0.24f, 320f, 460f);
            var x = Screen.width - width - 72f;
            var y = Screen.height - 178f;
            DrawRect(new Rect(x - 18f, y - 18f, width + 36f, 126f), new Color(0f, 0f, 0f, 0.42f));
            DrawRect(new Rect(x - 18f, y - 18f, 3f, 126f), new Color(0.78f, 0.54f, 0.22f, 0.66f));
            GUI.Label(new Rect(x, y, width, 24f), "THE FIRST JOB", _smallCapsStyle);
            DrawRule(new Rect(x, y + 30f, Mathf.Min(260f, width), 2f));
            GUI.Label(
                new Rect(x, y + 44f, width, 58f),
                "Walk to Luca, take the sedan, and reach the back office without dropping through the docks.",
                _bodyStyle);
        }

        private void DrawSodiumGlow(Rect rect)
        {
            const int steps = 18;
            for (var index = 0; index < steps; index += 1)
            {
                var t = index / (float)(steps - 1);
                var insetX = rect.width * 0.5f * t;
                var insetY = rect.height * 0.5f * t;
                var alpha = 0.10f * (1f - t);
                DrawRect(
                    new Rect(rect.x + insetX, rect.y + insetY, rect.width - (insetX * 2f), rect.height - (insetY * 2f)),
                    new Color(0.95f, 0.50f, 0.16f, alpha));
            }
        }

        private void DrawScreenRain()
        {
            const int streaks = 64;
            for (var index = 0; index < streaks; index += 1)
            {
                var seedX = Hash01(index * 17, 91);
                var seedY = Hash01(index * 29, 37);
                var x = seedX * Screen.width;
                var y = seedY * Screen.height;
                var length = 22f + (Hash01(index * 41, 13) * 46f);
                var alpha = 0.055f + (Hash01(index * 7, 19) * 0.055f);
                DrawRect(new Rect(x, y, 1.2f, length), new Color(0.54f, 0.63f, 0.68f, alpha));
            }
        }

        private void DrawHorizontalGradient(float startX, float height, float width, bool fadeRight, Color color)
        {
            const int steps = 18;
            var stepWidth = width / steps;
            for (var index = 0; index < steps; index += 1)
            {
                var t = index / (float)(steps - 1);
                var alpha = color.a * (fadeRight ? 1f - t : t);
                DrawRect(new Rect(startX + (index * stepWidth), 0f, stepWidth + 1f, height), new Color(color.r, color.g, color.b, alpha));
            }
        }

        private void DrawVerticalGradient(float startY, float width, float height, bool fadeDown, Color color)
        {
            const int steps = 14;
            var stepHeight = height / steps;
            for (var index = 0; index < steps; index += 1)
            {
                var t = index / (float)(steps - 1);
                var alpha = color.a * (fadeDown ? 1f - t : t);
                DrawRect(new Rect(0f, startY + (index * stepHeight), width, stepHeight + 1f), new Color(color.r, color.g, color.b, alpha));
            }
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
