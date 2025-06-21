using UnityEngine;

namespace Course.ScriptableObject.UnityServices
{
    [CreateAssetMenu(menuName = "Config/UnityServices", fileName = "UnityServicesConfig")]
    public class UnityServicesConfig : UnityEngine.ScriptableObject
    {
        [Header("Unity Ads")]
        [Tooltip("Your Android Game ID from the Unity dashboard")]
        [SerializeField]
        private string androidGameId;

        [Tooltip("Your iOS Game ID from the Unity dashboard")]
        [SerializeField]
        private string iOSGameId;

        [Tooltip("Set to true when testing in the Editor/Dev builds")]
        [field: SerializeField]
        public bool testMode { get; private set; } = true;

        public string GameId
        {
            get
            {
#if UNITY_IOS
            return iOSGameId;
#elif UNITY_ANDROID
                return androidGameId;
#else
            // In Editor (or any other unsupported platform) just return whichever,
            // but you could also throw or log a warning here.
            return testMode ? androidGameId : androidGameId;
#endif
            }
        }
    }
}