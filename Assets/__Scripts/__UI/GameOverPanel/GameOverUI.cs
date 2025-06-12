using System.Collections;
using Course.Attribute;
using Course.Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Course.UI
{
    public class GameOverUI : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("GUI Components")]
        [SerializeField]
        private GameObject[] gameOverObjects;
        
        [SerializeField]
        private TextMeshProUGUI levelText;
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI gameOverText;
        
        [Header("Settings")]
        [SerializeField, Tooltip("Seconds to wait before reloading the scene")]
        private float reloadDelay = 5f;
        
        [SerializeField] 
        private FadeCanvas fadeCanvas;

        #endregion

        #region Private Fields

        /// <summary>
        /// Interface for managing the score attribute behavior.
        /// </summary>
        private IAttributeBehaviour _score;

        /// <summary>
        /// Label text for displaying the final level.
        /// </summary>
        private const string LevelLabel = "Final Level: ";

        /// <summary>
        /// Label text for displaying the final score.
        /// </summary>
        private const string ScoreLabel = "Final Score: ";

        /// <summary>
        /// Stores the final level value.
        /// </summary>
        private ILevel _level;
        
        // Fractions of reloadDelay to spend in each phase
        private const float EFFECT_PHASE   = 0.3f;

        #endregion

        #region Public Method

        /// <summary>
        /// Initializes the GameOverUI with the specified score attribute and level.
        /// Subscribes to the OnValueChanged event and updates the UI.
        /// </summary>
        /// <param name="score">The score attribute behavior to bind to the UI.</param>
        /// <param name="level">The final level value to display.</param>
        public void Initialize(IAttributeBehaviour score, ILevel level)
        {
            _score = score;
            _level = level;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Updates the UI to reflect the current score and level.
        /// Formats the score value and sets the text for the level and score components.
        /// </summary>
        private void UpdateUI()
        {
            var displayValue = Mathf.Max(Mathf.RoundToInt(_score.Value), 0);
            scoreText.text = ScoreLabel + displayValue.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
            levelText.text = LevelLabel + _level.CurrentLevel;
        }

        /// <summary>
        /// Toggles the active state of all GameObjects in the `gameOverObjects` array.
        /// </summary>
        /// <param name="isActive">A boolean indicating whether the GameObjects should be active or inactive.</param>
        private void OnActive(bool isActive)
        {
            foreach(GameObject go in gameOverObjects)
            {
                go.SetActive(isActive);
            }
        }

        /// <summary>
        /// Coroutine that handles the reload process after a delay.
        /// Fades the screen, toggles the active state of UI elements, waits for the specified delay, 
        /// and reloads the current scene.
        /// </summary>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private IEnumerator ReloadAfterDelay()
        {
            OnActive(false);
            yield return fadeCanvas.FadeScreen(true, reloadDelay / 2, 1f);
    
            OnActive(true);
            UpdateUI();
            var textObjects = new TextMeshProUGUI[] { levelText, scoreText, gameOverText };
            yield return AnimateTextEffects(textObjects,reloadDelay * EFFECT_PHASE);
            
            yield return new WaitForSeconds(reloadDelay);
    
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
        
        private IEnumerator AnimateTextEffects(TextMeshProUGUI[] objs, float duration)
        {
            float t = 0f;
            // reset to initial
            foreach (var go in objs)
            {
                go.rectTransform.localScale = Vector3.zero;
                go.rectTransform.localEulerAngles = Vector3.zero;
                var c = go.color;
                c.a = 0f;
                go.color = c;
            }

            while (t < duration)
            {
                float u = Mathf.Clamp01(t / duration);
                float angle = 360f * 4f * u;
                float alpha       = u * u;
                float scale = u;     

                for (int i = 0; i < objs.Length; i++)
                {
                    var go = objs[i];
                    // apply spin
                    go.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
                    // apply fade
                    var c = go.color;
                    // apply scale
                    go.rectTransform.localScale = Vector3.one * scale;
                    c.a = alpha;
                    go.color = c;
                }

                t += Time.deltaTime;
                yield return null;
            }

            // ensure final state
            foreach (var go in objs)
            {
                go.rectTransform.localEulerAngles = Vector3.zero;
                go.rectTransform.localScale = Vector3.one;
                var c = go.color;
                c.a = 1f;
                go.color = c;
            }
        }

        #endregion

        #region Unity API

        /// <summary>
        /// Unity's OnEnable method, called when the GameObject becomes active.
        /// Updates the UI and starts the reload coroutine.
        /// </summary>
        void OnEnable()
        {
            StartCoroutine(ReloadAfterDelay());
        }

        #endregion
    }
}

