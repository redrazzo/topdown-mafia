using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Story;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private TopDownPlayerController? playerController;
        [SerializeField] private DialogueController? dialogueController;
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] private Rect menuRect = new Rect(24f, 24f, 420f, 250f);

        private GameSettingsData? _settings;
        private bool _showMenu;
        private bool _showSettings;

        private void Start()
        {
            _settings = GameSettingsFileService.LoadOrCreate();
            GameSettingsFileService.Apply(_settings);
        }

        private void OnDisable()
        {
            if (Mathf.Approximately(Time.timeScale, 0f))
            {
                Time.timeScale = 1f;
            }
        }

        private void Update()
        {
            if (dialogueController != null && dialogueController.IsActive)
            {
                return;
            }

            if (!Input.GetKeyDown(toggleKey))
            {
                return;
            }

            ToggleMenu();
        }

        private void OnGUI()
        {
            if (!_showMenu)
            {
                return;
            }

            if (_settings == null)
            {
                _settings = GameSettingsFileService.LoadOrCreate();
            }

            GUI.Box(menuRect, string.Empty);
            GUILayout.BeginArea(menuRect);
            GUILayout.Space(12f);
            GUILayout.Label("Paused");
            GUILayout.Space(12f);

            if (_showSettings)
            {
                DrawSettings();
            }
            else
            {
                if (GUILayout.Button("Resume", GUILayout.Height(34f)))
                {
                    ToggleMenu(forceOpen: false);
                }

                if (GUILayout.Button("Settings", GUILayout.Height(34f)))
                {
                    _showSettings = true;
                }

                if (GUILayout.Button("Return To Harbor Menu", GUILayout.Height(34f)))
                {
                    Time.timeScale = 1f;
                    if (playerController != null)
                    {
                        playerController.SetControlsLocked(false);
                    }

                    SceneManager.LoadScene("Boot");
                }
            }

            GUILayout.EndArea();
        }

        private void DrawSettings()
        {
            if (_settings == null)
            {
                return;
            }

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

        private void ToggleMenu(bool? forceOpen = null)
        {
            _showMenu = forceOpen ?? !_showMenu;
            _showSettings = false;
            Time.timeScale = _showMenu ? 0f : 1f;

            if (playerController != null)
            {
                playerController.SetControlsLocked(_showMenu);
            }
        }
    }
}
