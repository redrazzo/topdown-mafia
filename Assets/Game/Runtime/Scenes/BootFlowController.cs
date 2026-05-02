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
        [SerializeField] private Rect menuRect = new Rect(24f, 24f, 460f, 260f);
        [SerializeField] private string title = "Mafia Topdown City";
        [SerializeField] private string subtitle = "A quiet favor starts the fall.";

        private bool _saveExists;
        private bool _isLoading;
        private bool _showSettings;
        private GameSettingsData? _settings;
        private GUIStyle? _panelStyle;
        private GUIStyle? _titleStyle;
        private GUIStyle? _subtitleStyle;
        private GUIStyle? _bodyStyle;
        private GUIStyle? _buttonStyle;

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

            if (_isLoading)
            {
                GUI.Box(menuRect, GUIContent.none, _panelStyle);
                GUI.Label(new Rect(menuRect.x + 24f, menuRect.y + 26f, menuRect.width - 48f, 40f), "Loading...", _titleStyle);
                return;
            }

            GUI.Box(menuRect, GUIContent.none, _panelStyle);
            GUILayout.BeginArea(menuRect);
            GUILayout.Space(16f);
            GUILayout.Label(title, _titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(subtitle, _subtitleStyle);
            GUILayout.Space(18f);

            if (_showSettings)
            {
                DrawSettings();
            }
            else if (_saveExists)
            {
                GUILayout.Label("Continue the last job or start clean from the harbor.", _bodyStyle);
                GUILayout.Space(8f);
                if (GUILayout.Button("Continue [C]", _buttonStyle, GUILayout.Height(38f)))
                {
                    ContinueGame();
                }

                if (GUILayout.Button("New Game [N]", _buttonStyle, GUILayout.Height(38f)))
                {
                    StartNewGame();
                }

                if (GUILayout.Button("Settings [S]", _buttonStyle, GUILayout.Height(36f)))
                {
                    _showSettings = true;
                }
            }
            else
            {
                GUILayout.Label("Start a fresh run from the harbor district.", _bodyStyle);
                GUILayout.Space(8f);
                if (GUILayout.Button("Start New Game [N]", _buttonStyle, GUILayout.Height(38f)))
                {
                    StartNewGame();
                }

                if (GUILayout.Button("Settings [S]", _buttonStyle, GUILayout.Height(36f)))
                {
                    _showSettings = true;
                }
            }

            GUILayout.Space(8f);
            GUILayout.Label("WASD / E / Space. Esc opens the pause menu.", _bodyStyle);
            GUILayout.EndArea();
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

        private void DrawSettings()
        {
            if (_settings == null)
            {
                return;
            }

            GUILayout.Label("Settings", _subtitleStyle);
            GUILayout.Space(8f);
            GUILayout.Label("Master Volume", _bodyStyle);
            var updatedVolume = GUILayout.HorizontalSlider(_settings.MasterVolume, 0f, 1f);
            GUILayout.Label(Mathf.RoundToInt(updatedVolume * 100f) + "%", _bodyStyle);

            var updatedFullscreen = GUILayout.Toggle(_settings.Fullscreen, "Fullscreen", _bodyStyle);
            var updatedSubtitles = GUILayout.Toggle(_settings.Subtitles, "Subtitles", _bodyStyle);
            GUILayout.Label("HUD Scale", _bodyStyle);
            var updatedHudScale = GUILayout.HorizontalSlider(_settings.HudScale, 0.85f, 1.35f);
            GUILayout.Label(updatedHudScale.ToString("0.00") + "x", _bodyStyle);

            var changed =
                !Mathf.Approximately(updatedVolume, _settings.MasterVolume) ||
                updatedFullscreen != _settings.Fullscreen ||
                updatedSubtitles != _settings.Subtitles ||
                !Mathf.Approximately(updatedHudScale, _settings.HudScale);

            if (changed)
            {
                _settings.MasterVolume = updatedVolume;
                _settings.Fullscreen = updatedFullscreen;
                _settings.Subtitles = updatedSubtitles;
                _settings.HudScale = updatedHudScale;
                GameSettingsFileService.Save(_settings);
            }

            GUILayout.Space(12f);
            if (GUILayout.Button("Back", _buttonStyle, GUILayout.Height(36f)))
            {
                _showSettings = false;
            }
        }

        private void EnsureStyles()
        {
            if (_panelStyle != null)
            {
                return;
            }

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(18, 18, 18, 18)
            };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                normal = { textColor = new Color(0.94f, 0.86f, 0.69f, 1f) },
                wordWrap = false
            };

            _subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = new Color(0.72f, 0.61f, 0.43f, 1f) },
                wordWrap = true
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.84f, 0.8f, 0.72f, 1f) },
                wordWrap = true
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                normal = { textColor = new Color(0.92f, 0.85f, 0.71f, 1f) },
                hover = { textColor = new Color(1f, 0.92f, 0.72f, 1f) },
                active = { textColor = Color.white }
            };
        }

        private static void DrawBackdrop()
        {
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.035f, 0.04f, 0.05f, 1f));
            DrawRect(new Rect(0f, Screen.height * 0.64f, Screen.width, Screen.height * 0.36f), new Color(0.08f, 0.075f, 0.065f, 1f));
            DrawRect(new Rect(Screen.width * 0.55f, 0f, Screen.width * 0.09f, Screen.height), new Color(0.11f, 0.105f, 0.095f, 0.7f));
            DrawRect(new Rect(Screen.width * 0.59f, 0f, 3f, Screen.height), new Color(0.72f, 0.55f, 0.24f, 0.34f));
            DrawRect(new Rect(0f, 0f, Screen.width, 5f), new Color(0.67f, 0.49f, 0.22f, 1f));

            for (var index = 0; index < 9; index += 1)
            {
                var y = 52f + (index * 56f);
                DrawRect(new Rect(Screen.width * 0.69f, y, 38f, 12f), new Color(0.9f, 0.72f, 0.38f, 0.22f));
                DrawRect(new Rect(Screen.width * 0.18f, y + 24f, 24f, 8f), new Color(0.75f, 0.64f, 0.48f, 0.12f));
            }
        }

        private static void DrawRect(Rect rect, Color color)
        {
            var originalColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = originalColor;
        }
    }
}
