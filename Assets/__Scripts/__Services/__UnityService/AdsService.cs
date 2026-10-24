using System.Collections.Generic;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Course.Services.Unity
{
    public class AdsService: IAdsService, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private bool _initialized;
        private readonly Dictionary<string, bool> _isLoaded = new();

#if UNITY_IOS
        private const string InterstitialId = "Interstitial_iOS";
        private const string RewardedId     = "Rewarded_iOS";
        private const string BannerId       = "Banner_iOS";
#else
        private const string InterstitialId = "Interstitial_Android";
        private const string RewardedId     = "Rewarded_Android";
        private const string BannerId       = "Banner_Android";
#endif

        public bool IsInitialized => _initialized;

        #region IAdsService

        /// <summary>
        /// Call once (e.g. in your bootstrap Awake) to kick off Unity Ads.
        /// </summary>
        public void Initialize(string gameId, bool testMode)
        {
            if (_initialized || !Advertisement.isSupported) return;
            Advertisement.Initialize(gameId, testMode, this);
        }

        /// <summary>
        /// Loads an interstitial ad.  When it finishes loading,
        /// OnUnityAdsAdLoaded will be called.
        /// </summary>
        public void LoadInterstitial() => Load(InterstitialId);

        /// <summary>
        /// Shows an interstitial if it’s loaded.
        /// </summary>
        public void ShowInterstitial() => Show(InterstitialId);

        public void ShowBanner(string placementId = null)
        {
            placementId ??= BannerId;
            EventBus<GameStateChangedEvent>.Raise(
                new GameStateChangedEvent(GameState.paused)
            );
            if (_isLoaded.TryGetValue(placementId, out var ready) && ready)
            {
                ShowBannerWithOptions(placementId);
            }
            else
            {
                Debug.LogWarning($"[Ads] Banner not preloaded; loading now");
                Advertisement.Banner.Load(
                    placementId,
                    new BannerLoadOptions {
                        loadCallback  = () => ShowBannerWithOptions(placementId),
                        errorCallback = msg => {
                            Debug.LogError($"[Ads] Banner load failed: {msg}");
                            EventBus<GameStateChangedEvent>.Raise(
                                new GameStateChangedEvent(GameState.level)
                            );
                        }
                    }
                );
            }
        }
        
        public void LoadBanner(string placementId = null)
        {
            placementId ??= BannerId;
            if (_isLoaded.ContainsKey(placementId) && _isLoaded[placementId] || !_initialized) 
                return;

            Debug.Log($"[Ads] Loading banner: {placementId}");
            Advertisement.Banner.Load(
                placementId,
                new BannerLoadOptions {
                    loadCallback = () => {
                        Debug.Log($"[Ads] Banner loaded: {placementId}");
                        _isLoaded[placementId] = true;
                    },
                    errorCallback = msg => {
                        Debug.LogError($"[Ads] Banner failed to load: {msg}");
                        _isLoaded[placementId] = false;
                    }
                }
            );
        }

        /// <summary>
        /// Shows a rewarded ad if it’s loaded.
        /// </summary>
        public void ShowRewarded() => Debug.Log($"Rewarded ad shown (placeholder)");

        #endregion
        
        #region Private Methods
        
        private void Load(string placementId)
        {
            if (!_initialized) return;
            if (_isLoaded.ContainsKey(placementId) && _isLoaded[placementId]) return;
            Debug.Log($"[Ads] Loading placement: {placementId}");
            Advertisement.Load(placementId, this);
        }
        
        private void Show(string placementId)
        {
            if (_isLoaded.TryGetValue(placementId, out var ready) && ready)
            {
                EventBus<GameStateChangedEvent>.Raise(
                    new GameStateChangedEvent(GameState.paused)
                );    
                Debug.Log($"[Ads] Showing placement: {placementId}");
                Advertisement.Show(placementId, this);
            }
            else
            {
                Debug.LogWarning($"[Ads] Attempted to show '{placementId}' before it was loaded.");
            }
        }
        
        private void ShowBannerWithOptions(string placementId)
        {
            var options = new BannerOptions {
                showCallback  = () => Debug.Log("[Ads] Banner shown"),
                hideCallback  = () => {
                    Debug.Log("[Ads] Banner hidden");
                    EventBus<GameStateChangedEvent>.Raise(
                        new GameStateChangedEvent(GameState.level)
                    );
                },
                clickCallback = () => {
                    Debug.Log("[Ads] Banner clicked: granting reward");
                    ShowRewarded();

                    // if (_isLoaded.TryGetValue(RewardedId, out var ready) && ready)
                    //     Advertisement.Show(RewardedId, this);

                    EventBus<GameStateChangedEvent>.Raise(
                        new GameStateChangedEvent(GameState.level)
                    );
                }
            };

            Advertisement.Banner.Show(placementId, options);
        }

        #endregion

        #region IUnityAdsInitializationListener
        
        public void OnInitializationComplete()
        {
            _initialized = true;
            Debug.Log("[Ads] Initialization complete.");
            Advertisement.Load(InterstitialId, this);
            Advertisement.Load(RewardedId,     this);
            Advertisement.Banner.Load(BannerId);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"[Ads] Initialization failed: {error} - {message}");
        }

        #endregion

        #region IUnityAdsLoadListener
        
        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"[Ads] Loaded placement: {placementId}");
            _isLoaded[placementId] = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"[Ads] Failed to load '{placementId}': {error} - {message}");
            _isLoaded[placementId] = false;
        }

        #endregion

        #region IUnityAdsShowListener
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"[Ads] Failed to show '{placementId}': {error} - {message}");
            EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(GameState.level));
            _isLoaded[placementId] = false;
            Load(placementId);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"[Ads] Show started: {placementId}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"[Ads] Ad clicked: {placementId}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            EventBus<GameStateChangedEvent>.Raise(
                new GameStateChangedEvent(GameState.level)
            );
            Debug.Log($"[Ads] Show complete: {placementId} - {showCompletionState}");
            // For rewarded: only grant reward on COMPLETED
            if (placementId == RewardedId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                ShowRewarded();
                Debug.Log("[Ads] Reward the player!");
            }

            // After showing, mark not-loaded and reload
            _isLoaded[placementId] = false;
            Load(placementId);
        }

        #endregion
    }
}