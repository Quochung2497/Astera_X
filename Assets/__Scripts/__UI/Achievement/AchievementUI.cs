using System;
using System.Collections;
using Course.Core;
using Course.ScriptableObject;
using Course.Services.Achievements;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.DependencyInjection;

namespace Course.UI.Achievement
{
    public class AchievementUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI       title;
        [SerializeField] private TextMeshProUGUI       description;
        [SerializeField] private RectTransform  panelRect;    
        
        [Header("Timing (in seconds)")]
        [SerializeField] private float slideDuration  = 0.5f;  // time to slide in/out
        [SerializeField] private float displayDuration = 2f;   // how long to stay visible
    
        private Vector2 _hiddenPos;
        private Vector2 _visiblePos;
        private Coroutine _popupRoutine;
        
        [Inject]
        private AchievementManager _achievementManager;
    
        private void Awake()
        {
            if(_achievementManager == null)
            {
                Debug.LogError("AchievementManager not found. Make sure it is present in the scene.");
                return;
            }
            // cache the “on‐screen” position
            _visiblePos = panelRect.anchoredPosition;
            // hidden = same x, but y shifted up by panel height
            _hiddenPos  = _visiblePos + Vector2.up * panelRect.rect.height;
            // start hidden
            panelRect.anchoredPosition = _hiddenPos;
        }
        
        private void OnEnable()
        {
            _achievementManager.OnShowAchievement += HandleShow;
        }

        private void OnDisable()
        {
            _achievementManager.OnShowAchievement -= HandleShow;
        }
        
        private void HandleShow(AchievementSO def)
        {
            Show(def.Title, def.Description);
        }
    
        /// <summary>
        /// Triggers the panel to pop in, show your text, then slide back out.
        /// </summary>
        private void Show(string title, string description)
        {
            // If we’re mid‐popup, cancel it
            if (_popupRoutine != null) StopCoroutine(_popupRoutine);
            _popupRoutine = StartCoroutine(PopupSequence(title, description));
        }
    
        private IEnumerator PopupSequence(string title, string description)
        {
            yield return new WaitUntil(() =>
                GameManager.TryGetInstance().currentStates == GameState.level
            );
            // fill in text
            this.title.text       = title;
            this.description.text = description;
    
            // 1) Slide in
            yield return Slide(_hiddenPos, _visiblePos);
    
            // 2) Wait
            yield return new WaitForSecondsRealtime(displayDuration);
    
            // 3) Slide out
            yield return Slide(_visiblePos, _hiddenPos);
            _achievementManager?.Acknowledge();
        }
    
        private IEnumerator Slide(Vector2 from, Vector2 to)
        {
            float t = 0f;
            while (t < slideDuration)
            {
                t += Time.unscaledDeltaTime;
                float frac = Mathf.Clamp01(t / slideDuration);
                panelRect.anchoredPosition = Vector2.Lerp(from, to, frac);
                yield return null;
            }
            panelRect.anchoredPosition = to;
        }
    }
}
