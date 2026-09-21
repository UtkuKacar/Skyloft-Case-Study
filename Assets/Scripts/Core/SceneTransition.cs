using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Skyloft.Core
{
    /// <summary>One persistent overlay responsible only for fading and loading scenes.</summary>
    public sealed class SceneTransition : MonoBehaviour
    {
        public const string MenuScene = "MainMenu";
        public const string GameScene = "Game";
        public const string ResultScene = "Result";
        private const float FadeDuration = 0.35f;
        public static SceneTransition Instance { get; private set; }
        public bool IsTransitioning { get; private set; }
        public float FadeAlpha => overlay.alpha;
        private CanvasGroup overlay;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null)
                return;
            var root = new GameObject("SceneTransition");
            root.AddComponent<SceneTransition>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;
            gameObject.AddComponent<GraphicRaycaster>();
            overlay = gameObject.AddComponent<CanvasGroup>();
            var image = new GameObject("Black", typeof(RectTransform), typeof(Image));
            image.transform.SetParent(transform, false);
            var rect = (RectTransform)image.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            image.GetComponent<Image>().color = Color.black;
            overlay.alpha = 1f;
            overlay.blocksRaycasts = true;
            IsTransitioning = true;
        }

        private IEnumerator Start()
        {
            yield return Fade(0f);
            overlay.blocksRaycasts = false;
            IsTransitioning = false;
        }

        public void Load(string scene)
        {
            if (!IsTransitioning)
                StartCoroutine(ChangeScene(scene));
        }

        private IEnumerator ChangeScene(string scene)
        {
            IsTransitioning = true;
            overlay.blocksRaycasts = true;
            yield return Fade(1f);
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return Fade(0f);
            overlay.blocksRaycasts = false;
            IsTransitioning = false;
        }

        private IEnumerator Fade(float target)
        {
            while (!Mathf.Approximately(overlay.alpha, target))
            {
                yield return null;
                // Do not consume the frame that loaded the scene; retain visible steps after a loading hitch.
                float step = Mathf.Min(Time.unscaledDeltaTime, 1f / 15f) / FadeDuration;
                overlay.alpha = Mathf.MoveTowards(overlay.alpha, target, step);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
