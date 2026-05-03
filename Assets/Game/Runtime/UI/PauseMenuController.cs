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
        [SerializeField] private Rect menuRect = new Rect(0f, 0f, 460f, 315f);

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

            var rect = HudStyleUtility.CenteredRect(menuRect.width, _showSettings ? 470f : menuRect.height);
            HudStyleUtility.DrawMenuFrame(
                rect,
                _showSettings ? "Settings" : "Paused",
                _showSettings ? "Tune the picture, sound, and readability before you step back into the street." : "The city keeps breathing. Choose your next move.");

            GUILayout.BeginArea(new Rect(rect.x + 22f, rect.y + 102f, rect.width - 44f, rect.height - 124f));

            if (_showSettings)
            {
                DrawSettings();
            }
            else
            {
                if (GUILayout.Button("Resume", HudStyleUtility.ButtonStyle, GUILayout.Height(36f)))
                {
                    ToggleMenu(forceOpen: false);
                }

                GUILayout.Space(8f);
                if (GUILayout.Button("Settings", HudStyleUtility.ButtonStyle, GUILayout.Height(36f)))
                {
                    _showSettings = true;
                }

                GUILayout.Space(8f);
                if (GUILayout.Button("Return To Harbor Menu", HudStyleUtility.ButtonStyle, GUILayout.Height(36f)))
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

            GUILayout.Label("Master Volume", HudStyleUtility.MenuBodyStyle);
            var updatedVolume = GUILayout.HorizontalSlider(_settings.MasterVolume, 0f, 1f);
            GUILayout.Label(Mathf.RoundToInt(updatedVolume * 100f) + "%", HudStyleUtility.MenuBodyStyle);
            var previousResolutionWidth = _settings.ResolutionWidth;
            var previousResolutionHeight = _settings.ResolutionHeight;

            GUILayout.Space(8f);
            var updatedFullscreen = GUILayout.Toggle(_settings.Fullscreen, "Fullscreen", HudStyleUtility.MenuBodyStyle);
            var updatedSubtitles = GUILayout.Toggle(_settings.Subtitles, "Subtitles", HudStyleUtility.MenuBodyStyle);

            GUILayout.Space(8f);
            if (GUILayout.Button("Resolution: " + _settings.ResolutionWidth + " x " + _settings.ResolutionHeight, HudStyleUtility.ButtonStyle, GUILayout.Height(34f)))
            {
                GameSettingsFileService.CycleResolution(_settings);
            }

            GUILayout.Space(8f);
            GUILayout.Label("HUD Scale", HudStyleUtility.MenuBodyStyle);
            var updatedHudScale = GUILayout.HorizontalSlider(_settings.HudScale, 0.85f, 1.35f);
            GUILayout.Label(updatedHudScale.ToString("0.00") + "x", HudStyleUtility.MenuBodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("Input Sensitivity", HudStyleUtility.MenuBodyStyle);
            var updatedInputSensitivity = GUILayout.HorizontalSlider(_settings.InputSensitivity, 0.75f, 1.35f);
            GUILayout.Label(updatedInputSensitivity.ToString("0.00") + "x", HudStyleUtility.MenuBodyStyle);

            var changed =
                !Mathf.Approximately(updatedVolume, _settings.MasterVolume) ||
                updatedFullscreen != _settings.Fullscreen ||
                updatedSubtitles != _settings.Subtitles ||
                !Mathf.Approximately(updatedHudScale, _settings.HudScale) ||
                !Mathf.Approximately(updatedInputSensitivity, _settings.InputSensitivity) ||
                previousResolutionWidth != _settings.ResolutionWidth ||
                previousResolutionHeight != _settings.ResolutionHeight;

            if (changed)
            {
                _settings.MasterVolume = updatedVolume;
                _settings.Fullscreen = updatedFullscreen;
                _settings.Subtitles = updatedSubtitles;
                _settings.HudScale = updatedHudScale;
                _settings.InputSensitivity = updatedInputSensitivity;
                GameSettingsFileService.Save(_settings);
            }

            GUILayout.Space(12f);
            if (GUILayout.Button("Back", HudStyleUtility.ButtonStyle, GUILayout.Height(36f)))
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
