using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Course.UI
{
    public class FadeCanvas : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Image component used for fading the screen.
        /// Serialized field to allow assignment in the Unity Inspector.
        /// </summary>
        [SerializeField]
        private Image fadeImage;

        #region Public Method

        /// <summary>
        /// Coroutine to fade the screen in or out over a specified duration.
        /// </summary>
        /// <param name="fadeIn">If true, fades the screen in; otherwise, fades the screen out.</param>
        /// <param name="duration">The duration of the fade effect in seconds.</param>
        /// <param name="targetAlpha">The target alpha value for the fade effect.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        public IEnumerator FadeScreen(bool fadeIn, float duration, float targetAlpha)
        {
            float startAlpha = fadeIn ? 0f : fadeImage.color.a;
            float endAlpha = fadeIn ? targetAlpha : 0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadeImage.color = new Color(0, 0, 0, endAlpha);
        }

        #endregion

        #region Unity API

        /// <summary>
        /// Unity's Awake method.
        /// Ensures the fadeImage is assigned and initializes its properties.
        /// </summary>
        private void Awake()
        {
            if (fadeImage == null)
            {
                Debug.LogError("fadeImage is not assigned.");
                return;
            }

            fadeImage.color = new Color(0, 0, 0, 1);
            fadeImage.raycastTarget = false;
        }

        /// <summary>
        /// Unity's Start method.
        /// Begins the fade-out effect at the start of the scene.
        /// </summary>
        private void Start()
        {
            StartCoroutine(FadeScreen(false, 1f, 0f));
        }

        #endregion

    }
}
