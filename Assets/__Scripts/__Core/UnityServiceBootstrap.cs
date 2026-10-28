using System;
using System.Collections.Generic;
using Course.Control;
using Course.ScriptableObject;
using Course.ScriptableObject.UnityServices;
using Course.Services.Achievements;
using Course.Services.Unity;
using Course.Utility;
using Course.Utility.Events;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.DependencyInjection;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace Course.Core
{
    public class UnityServiceBootstrap : PrivateSingleton<UnityServiceBootstrap>
    {
        private int   _currentLevel;
        private int   _lastScore;
        private bool  _didHitHighThisSession;

        private EventBinding<LevelReached>         _levelEvent;
        private EventBinding<ScoreUpdated>         _scoreEvent;
        private EventBinding<GameStateChangedEvent> _gameOverEvent;
        private IAdsService _adsService;

        // TODO: Testing for now
        private Vector2 _restingTilt;

        [SerializeField] private AsteroidsConfig asteroidsConfig;
        [SerializeField] private UnityServicesConfig unityServicesConfig;
        [SerializeField] private InputReader inputReader;
        
        [Inject] private IAchievementManager _achievementManager;
        [Inject] private ICustomization _customization;

        public void RunAds()
        {
            if (_adsService?.IsInitialized != true)
                return;
            _adsService.LoadInterstitial();
            _adsService.ShowInterstitial();
        }

        public void RunBanner()
        {
            if (_adsService?.IsInitialized != true)
                return;
            _adsService.LoadBanner();
            _adsService.ShowBanner();
        }
        
        #region Unity Life Cycle

        protected override async void Awake()
        {
            base.Awake();
            _adsService = new AdsService();
            if (unityServicesConfig != null)
                _adsService.Initialize(unityServicesConfig.GameId, unityServicesConfig.testMode);

            try
            {
                await UnityAnalytics.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Analytics] Initialization failed; gameplay remains local: {e.Message}");
            }

            try
            {
                await UnityRemoteConfig.InitializeAsync();
                UnityRemoteConfig.FetchCompleted += OnRemoteConfigFetched;
                UnityRemoteConfig.FetchConfigs();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[RC] Initialization failed; keeping local spawn rules: {e.Message}");
            }
        }

        private void Update()
        {
            if (Accelerometer.current != null && Accelerometer.current.enabled)
                PollAccelerometer();
        }

        private void OnEnable()
        {
            EnableSensors();

            RegisterLevelEvents();
            RegisterScoreEvents();
            RegisterAchievementEvents();
            RegisterGameOverEvents();
        }

        private void OnDisable()
        {
            DeregisterLevelEvents();
            DeregisterScoreEvents();
            DeregisterAchievementEvents();
            DeregisterGameOverEvents();
                        
            if (Accelerometer.current != null)
                InputSystem.DisableDevice(Accelerometer.current);
            if (Gyroscope.current != null)
                InputSystem.DisableDevice(Gyroscope.current);
        }

        private void Start()
        {
            // TODO: Testing for now
            Vector3 a = Accelerometer.current?.acceleration.ReadValue() ?? Vector3.zero;
            _restingTilt = new Vector2(a.x, a.y);
        }

        private void OnDestroy()
        {
            UnityRemoteConfig.FetchCompleted -= OnRemoteConfigFetched;
        }

        #endregion
        
        #region LevelStart

        private void RegisterLevelEvents()
        {
            _levelEvent = new EventBinding<LevelReached>(OnLevelReached);
            EventBus<LevelReached>.Register(_levelEvent);
        }

        private void DeregisterLevelEvents()
        {
            EventBus<LevelReached>.Deregister(_levelEvent);
        }

        private void OnLevelReached(LevelReached e)
        {
            _currentLevel = e.LevelNumber;
            UnityAnalytics.LogLevelStart(e.LevelNumber);
        }

        #endregion

        #region ScoreTracking

        private void RegisterScoreEvents()
        {
            _scoreEvent = new EventBinding<ScoreUpdated>(OnScoreUpdated);
            EventBus<ScoreUpdated>.Register(_scoreEvent);
        }

        private void DeregisterScoreEvents()
        {
            EventBus<ScoreUpdated>.Deregister(_scoreEvent);
        }

        private void OnScoreUpdated(ScoreUpdated e)
        {
            _lastScore = (int)e.TotalScore;
        }

        #endregion

        #region AchievementUnlocked

        private void RegisterAchievementEvents()
        {
            if (_achievementManager != null)
                _achievementManager.OnShowAchievement += OnShowAchievement;
        }

        private void DeregisterAchievementEvents()
        {
            if (_achievementManager != null)
                _achievementManager.OnShowAchievement -= OnShowAchievement;
        }

        private void OnShowAchievement(AchievementSO def)
        {
            UnityAnalytics.LogAchievementUnlocked(def.Title);
        }

        #endregion

        #region GameOver

        private void RegisterGameOverEvents()
        {
            _gameOverEvent = new EventBinding<GameStateChangedEvent>(OnGameOver);
            EventBus<GameStateChangedEvent>.Register(_gameOverEvent);
        }

        private void DeregisterGameOverEvents()
        {
            EventBus<GameStateChangedEvent>.Deregister(_gameOverEvent);
        }

        private void OnGameOver(GameStateChangedEvent e)
        {
            if (e.NewState != GameState.gameOver) return;

            bool gotHigh = _achievementManager?.DidHitNewHighThisSession ?? false;
            UnityAnalytics.LogGameOver(_lastScore, _currentLevel, gotHigh);
            UnityAnalytics.LogShipPartChoice(
                _customization.SelectedBodyIndex,
                _customization.SelectedTurretIndex);
        }

        #endregion

        #region RemoteConfig

        private void OnRemoteConfigFetched(ConfigResponse resp)
        {
            if (asteroidsConfig == null || resp.requestOrigin == ConfigOrigin.Default)
                return;

            try
            {
                string json = RemoteConfigService.Instance.appConfig.GetJson("levelSpawnRules");
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var wrapper = JsonUtility.FromJson<LevelSpawnRuleArrayWrapper>($"{{\"items\":{json}}}");
                if (wrapper?.items == null || wrapper.items.Length == 0)
                    throw new ArgumentException("levelSpawnRules must contain at least one level.");

                var levels = new HashSet<int>();
                foreach (var level in wrapper.items)
                {
                    var rule = level.rule;
                    if (level.levelNumber < 0 || !levels.Add(level.levelNumber) ||
                        rule.parentSize < 1 || rule.childSize < 1 || rule.grandchildSize < 1 ||
                        rule.parentCount < 0 || rule.childCount < 0 || rule.grandchildCount < 0 ||
                        rule.parentHealth < 1 || rule.childHealth < 1 || rule.grandchildHealth < 1 ||
                        rule.parentPoints < 0 || rule.childPoints < 0 || rule.grandchildPoints < 0 ||
                        rule.parentDamage < 0 || rule.childDamage < 0 || rule.grandchildDamage < 0)
                        throw new ArgumentException("levelSpawnRules contains invalid or duplicate level data.");
                }

                asteroidsConfig.SetLevelSpawnRules(wrapper.items);
                Debug.Log($"[RC] Applied {wrapper.items.Length} spawn rules from {resp.requestOrigin}.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[RC] Keeping local spawn rules: {e.Message}");
            }
        }

        [Serializable]
        private class LevelSpawnRuleArrayWrapper
        {
            public AsteroidsConfig.LevelSpawnRule[] items;
        }

        #endregion

        private void EnableSensors()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Accelerometer.current == null)
                InputSystem.AddDevice<Accelerometer>();
            if (Gyroscope.current == null)
                InputSystem.AddDevice<Gyroscope>();
#endif

            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
            if (Gyroscope.current != null)
                InputSystem.EnableDevice(Gyroscope.current);

            foreach (var device in InputSystem.devices)
                Debug.Log($"Device: {device}");
        }
        
        public void PollAccelerometer()
        {
            // 1) read raw 3D accel
            Vector3 a = Accelerometer.current?.acceleration.ReadValue() ?? Vector3.zero;

            // 2) subtract your baseline resting tilt
            Vector2 delta = new Vector2(a.x, a.y) - _restingTilt;

            // 3) find the signed tilt angle around X–axis:
            //    use Atan2(y,z) if you consider 'y' pointing 
            //    up/down and 'z' forward/back in device space.
            float tiltX = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            //    Now tiltX is –180…+180, zero when level.

            // 4) clamp into ±90° so “over-tilt” just saturates
            float tiltClamped = Mathf.Clamp(tiltX, -90f, +90f);

            // 5) remap to –1…+1
            float normY = tiltClamped / 90f;
            
            // normY *= 0.8f; // optional: scale down to 80% of full range

            // 6) use your existing X mapping
            float normX = delta.x; // or delta.x / maxTiltX

            // 7) apply your deadzone/sensitivity
            Vector2 raw = new Vector2(normX, normY);
            Vector2 move = ApplyDeadzone(raw, 0.1f);

            inputReader.FeedMove(move);
        }
        
        private Vector2 ApplyDeadzone(Vector2 v, float minDead, float maxDead = 1f)
        {
            float m = v.magnitude;
            if (m < minDead) return Vector2.zero;
            if (m > maxDead) return v.normalized;

            // remap [minDead…maxDead] → [0…1]
            float t = (m - minDead) / (maxDead - minDead);
            return v.normalized * t;
        }
    }
}
