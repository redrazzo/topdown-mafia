using System.Collections;
using MafiaTopDown.Gameplay.Domain.Scenes;
using MafiaTopDown.Gameplay.Runtime.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Gameplay.Runtime.Scenes
{
    public sealed class SceneTransitionController : MonoBehaviour
    {
        [SerializeField] private SaveGameFileService saveGameFileService;
        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private float fadeDuration = 0.35f;

        public void TransitionToExteriorInteriorPair(string sourceScene, string targetScene, string spawnPointId)
        {
            var request = new SceneTransitionRequest(sourceScene, targetScene, spawnPointId, SceneFadeMode.FadeToBlack);
            StartCoroutine(TransitionRoutine(request));
        }

        public IEnumerator TransitionRoutine(SceneTransitionRequest request)
        {
            yield return Fade(1f);
            if (saveGameFileService != null)
            {
                saveGameFileService.UpdateSceneLocation(request.TargetScene, request.SpawnPointId);
            }

            yield return SceneManager.LoadSceneAsync(request.TargetScene);
            yield return Fade(0f);
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (fadeCanvas == null)
            {
                yield break;
            }

            fadeCanvas.blocksRaycasts = targetAlpha > 0f;
            var startAlpha = fadeCanvas.alpha;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvas.alpha = targetAlpha;
        }
    }
}
