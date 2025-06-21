namespace Course.Services.Unity
{
    public interface IAdsService
    {
        /// <summary>Initialize the SDK with your platform game-ID.</summary>
        void Initialize(string gameId, bool testMode);

        /// <summary>True once initialization has completed successfully.</summary>
        bool IsInitialized { get; }

        /// <summary>Show an interstitial ad if ready.</summary>
        void ShowInterstitial();

        /// <summary>Show a banner ad at the given placement.</summary>
        void ShowBanner(string placementId = null);
        
        void LoadInterstitial();
        
        void LoadBanner(string placementId = null);
        
        void ShowRewarded();
    }
}