using UnityEngine;
using UnityEngine.UI;

namespace MafiaTopDown.Gameplay.Runtime.UI
{
    [DisallowMultipleComponent]
    public sealed class NoirScreenGradeController : MonoBehaviour
    {
        [SerializeField] private Color tintColor = new Color(0.03f, 0.045f, 0.065f, 0.18f);
        [SerializeField] private Color vignetteColor = new Color(0f, 0f, 0f, 0.62f);
        [SerializeField] private float letterboxHeight = 0.055f;
        [SerializeField] private int vignetteTextureSize = 256;

        private void Start()
        {
            var canvasObject = new GameObject("NoirScreenGradeOverlay");
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1000;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            AddPanel(canvas.transform, "BlueSmokeTint", tintColor, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            AddImage(canvas.transform, "NoirVignette", CreateVignetteSprite(), Color.white, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            AddPanel(canvas.transform, "TopLetterbox", new Color(0f, 0f, 0f, 0.72f), new Vector2(0f, 1f - letterboxHeight), Vector2.one, Vector2.zero, Vector2.zero);
            AddPanel(canvas.transform, "BottomLetterbox", new Color(0f, 0f, 0f, 0.72f), Vector2.zero, new Vector2(1f, letterboxHeight), Vector2.zero, Vector2.zero);
        }

        private static void AddPanel(
            Transform parent,
            string name,
            Color color,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            AddImage(parent, name, null, color, anchorMin, anchorMax, offsetMin, offsetMax);
        }

        private static void AddImage(
            Transform parent,
            string name,
            Sprite sprite,
            Color color,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.sprite = sprite;
            image.raycastTarget = false;

            var rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }

        private Sprite CreateVignetteSprite()
        {
            var size = Mathf.Max(64, vignetteTextureSize);
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "RuntimeNoirVignette",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            var maxDistance = center.magnitude;
            for (var y = 0; y < size; y += 1)
            {
                for (var x = 0; x < size; x += 1)
                {
                    var distance = Vector2.Distance(new Vector2(x, y), center) / maxDistance;
                    var alpha = Mathf.SmoothStep(0.06f, vignetteColor.a, Mathf.InverseLerp(0.35f, 1f, distance));
                    texture.SetPixel(x, y, new Color(vignetteColor.r, vignetteColor.g, vignetteColor.b, alpha));
                }
            }

            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
