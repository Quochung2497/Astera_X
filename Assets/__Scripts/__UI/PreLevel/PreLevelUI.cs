using System;
using System.Collections;
using Course.Core;
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
        private IAsteraX _asteraX;
        private int _asteroidCount;
        private int _childCount;
        private int _grandchildCount;
        private bool _isInitialized;

        /// <summary>
        /// Call this once *before* enabling the panel:
        ///
        ///   preUI.Initialize(levelController, levelNumber, parentCount, childCount, grandchildCount);
        ///   preUI.gameObject.SetActive(true);
        ///
        /// This will store the data, then call UpdateUI() → so that when OnEnable() runs,
        /// both text fields are already populated, and then the flip-Y animation begins.
        /// </summary>
        public void Initialize(
            ILevel levelController,
            IAsteraX asteraX
        )
        {
            _asteraX = asteraX;
            _levelController  = levelController;
            UpdateUI();
            _levelController.OnPreLevel += UpdateUI;
            _isInitialized = true;
        }

        /// <summary>
        /// Assigns text into both labels: “Level X” and “Asteroids: A  Children: C  Grandchildren: G”.
        /// Called by Initialize() (before enabling) so that the moment OnEnable() runs, the labels exist.
        /// </summary>
        private void UpdateUI()
        {
            if (levelPanel != null)
            {
                levelPanel.text = $"Level {_levelController.CurrentLevel}";
            }
            
            if (_asteraX.asteroidsSO.TryGetSpawnRuleForLevel(_levelController.CurrentLevel, out var rule))
            {
                _asteroidCount   = rule.parentCount;
                _childCount      = rule.parentCount * rule.childCount;
                _grandchildCount = rule.parentCount * rule.childCount * rule.grandchildCount;
            }
            else
            {
                // No rule → maybe set all to zero
                _asteroidCount   = 0;
                _childCount      = 0;
                _grandchildCount = 0;
            }

            if (infoLabel != null)
            {
                infoLabel.text =
                    $"Asteroids: {_asteroidCount}   " +
                    $"Children: {_childCount}   " +
                    $"Grandchildren: {_grandchildCount}";
            }
        }

        /// <summary>
        /// As soon as this GameObject (the pre-level panel) becomes active,
        /// kick off the flip-Y + fade + scale animation on both `levelPanel` and `infoLabel`.
        /// Once that animation is done, call StartNextLevel() on the controller and hide ourselves.
        /// </summary>
        private void OnEnable()
        {
            if (levelPanel == null || infoLabel == null)
            {
                Debug.LogWarning("[PreLevelUI] One or both TextMeshProUGUI references are missing!");
                return;
            }
            StartCoroutine(FlipY_AnimateThenBeginLevel());
        }

        private void OnDestroy()
        {
            if (!_isInitialized) return;
            _levelController.OnPreLevel -= UpdateUI;
        }

        /// <summary>
        /// Plays a synchronized flip-Y + fade + scale animation over `animationDuration`
        /// on both text fields. When finished, invokes StartNextLevel() and disables this panel.
        /// </summary>
        private IEnumerator FlipY_AnimateThenBeginLevel()
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

            _levelController?.StartNextLevel();
        }
    }
}

