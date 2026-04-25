using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class BootFlowController : MonoBehaviour
    {
        [SerializeField] private string firstPlayableScene = "District_01";
        [SerializeField] private SaveGameFileService saveGameFileService;
        [SerializeField] private bool loadLastSceneFromSave = true;
        [SerializeField] private Rect menuRect = new Rect(24f, 24f, 460f, 260f);
        [SerializeField] private string title = "Mafia Topdown City";
        [SerializeField] private string subtitle = "A quiet favor starts the fall.";

        private bool _saveExists;
        private bool _isLoading;
        private bool _showSettings;
        private GameSettingsData? _settings;

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
            if (_isLoading)
            {
                GUI.Box(menuRect, "Loading...");
                return;
            }

            GUI.Box(menuRect, string.Empty);
            GUILayout.BeginArea(menuRect);
            GUILayout.Space(12f);
            GUILayout.Label(title);
            GUILayout.Space(4f);
            GUILayout.Label(subtitle);
            GUILayout.Space(18f);

            if (_showSettings)
            {
                DrawSettings();
            }
            else if (_saveExists)
            {
                GUILayout.Label("Continue your last save or start clean from the harbor.");
                GUILayout.Space(8f);
                if (GUILayout.Button("Continue [C]", GUILayout.Height(36f)))
                {
                    ContinueGame();
                }

                if (GUILayout.Button("New Game [N]", GUILayout.Height(36f)))
                {
                    StartNewGame();
                }

                if (GUILayout.Button("Settings [S]", GUILayout.Height(34f)))
                {
                    _showSettings = true;
                }
            }
            else
            {
                GUILayout.Label("Start a fresh run from the harbor district.");
                GUILayout.Space(8f);
                if (GUILayout.Button("Start New Game [N]", GUILayout.Height(36f)))
                {
                    StartNewGame();
                }

                if (GUILayout.Button("Settings [S]", GUILayout.Height(34f)))
                {
                    _showSettings = true;
                }
            }

            GUILayout.Space(8f);
            GUILayout.Label("Keyboard: WASD / E / Space. Gamepad also supported.");
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
            if (loadLastSceneFromSave)
            {
                var save = saveGameFileService.LoadOrCreateSave();
                if (!string.IsNullOrWhiteSpace(save.LastSceneName))
                {
                    targetScene = save.LastSceneName;
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

            GUILayout.Label("Settings");
            GUILayout.Space(8f);
            GUILayout.Label("Master Volume");
            var updatedVolume = GUILayout.HorizontalSlider(_settings.MasterVolume, 0f, 1f);
            GUILayout.Label(Mathf.RoundToInt(updatedVolume * 100f) + "%");

            var updatedFullscreen = GUILayout.Toggle(_settings.Fullscreen, "Fullscreen");
            var updatedSubtitles = GUILayout.Toggle(_settings.Subtitles, "Subtitles");
            GUILayout.Label("HUD Scale");
            var updatedHudScale = GUILayout.HorizontalSlider(_settings.HudScale, 0.85f, 1.35f);
            GUILayout.Label(updatedHudScale.ToString("0.00") + "x");

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
            if (GUILayout.Button("Back", GUILayout.Height(34f)))
            {
                _showSettings = false;
            }
        }
    }
}
