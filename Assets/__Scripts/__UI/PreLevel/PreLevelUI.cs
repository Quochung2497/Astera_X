using System;
using System.Collections;
using Course.Level;
using TMPro;
using UnityEngine;

namespace Course.UI
{
    public class PreLevelUI : MonoBehaviour
    {
        [Header("Text Fields (assign in Inspector)")]
        [Tooltip("The separate label that shows \"Level X\"")]
        [SerializeField]
        private TextMeshProUGUI levelPanel = null;

        [Tooltip("The single line that shows “Asteroids: ...   Children: ...   Grandchildren: ...”")]
        [SerializeField]
        private TextMeshProUGUI infoLabel = null;

        [Header("Animation Settings")]
        [Tooltip("How many seconds to spend on flip-Y + fade + scale animation")]
        [SerializeField]
        private float animationDuration = 2f;

        [Header("Prelevel Time Setting")] 
        [SerializeField]
        private float duration = 2f;

        private ILevel _levelController;
        private Action _onPresentationCompleted;
        private bool _isInitialized;

        /// <summary>
        /// Binds the level data and the callback invoked after the presentation completes.
        /// </summary>
        public void Initialize(ILevel levelController, Action onPresentationCompleted)
        {
            _levelController = levelController;
            _onPresentationCompleted = onPresentationCompleted;
            _levelController.OnPreLevel += UpdateUI;
            _isInitialized = true;
        }

        /// <summary>
        /// Assigns text into both labels: “Level X” and “Asteroids: A  Children: C  Grandchildren: G”.
        /// Called after the rule is selected and before the panel is enabled.
        /// </summary>
        private void UpdateUI()
        {
            if (levelPanel != null)
            {
                levelPanel.text = $"Level {_levelController.CurrentLevel}";
            }
            
            if (infoLabel == null) return;

            var rule = _levelController.CurrentSpawnRule.GetValueOrDefault();
            int childCount = rule.parentCount * rule.childCount;
            infoLabel.text =
                $"Asteroids: {rule.parentCount}   " +
                $"Children: {childCount}   " +
                $"Grandchildren: {childCount * rule.grandchildCount}";
        }

        /// <summary>
        /// As soon as this GameObject (the pre-level panel) becomes active,
        /// kick off the flip-Y + fade + scale animation on both `levelPanel` and `infoLabel`.
        /// Reports completion after the animation finishes.
        /// </summary>
        private void OnEnable()
        {
            if (levelPanel == null || infoLabel == null)
            {
                Debug.LogWarning("[PreLevelUI] One or both TextMeshProUGUI references are missing!");
                return;
            }
            StartCoroutine(PlayPresentation());
        }

        private void OnDestroy()
        {
            if (!_isInitialized) return;
            _levelController.OnPreLevel -= UpdateUI;
        }

        /// <summary>
        /// Plays a synchronized flip-Y + fade + scale animation over `animationDuration`
        /// on both text fields, then invokes the presentation completion callback.
        /// </summary>
        private IEnumerator PlayPresentation()
        {
            TextMeshProUGUI[] texts = new TextMeshProUGUI[] { levelPanel, infoLabel };
            foreach (var txt in texts)
            {
                txt.rectTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }

            float t = 0f;
            while (t < animationDuration)
            {
                float u = Mathf.Clamp01(t / animationDuration);

                float flipAngle = 90f * (1f - u);

                foreach (var txt in texts)
                {
                    // a) rotate around Y from 90 → 0
                    txt.rectTransform.localEulerAngles = new Vector3(flipAngle, 0f, 0f);
                }

                t += Time.deltaTime;
                yield return null;
            }

            foreach (var txt in texts)
            {
                txt.rectTransform.localEulerAngles = Vector3.zero;
            }

            yield return new WaitForSeconds(duration);
            
            t = 0f;
            while (t < animationDuration)
            {
                float u = Mathf.Clamp01(t / animationDuration);
                
                float flipAngle = 90f * u;  
   

                foreach (var txt in texts)
                {
                    txt.rectTransform.localEulerAngles = new Vector3(flipAngle, 0f, 0f);
                }

                t += Time.deltaTime;
                yield return null;
            }

            foreach (var txt in texts)
            {
                txt.rectTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }

            _onPresentationCompleted?.Invoke();
        }
    }
}

