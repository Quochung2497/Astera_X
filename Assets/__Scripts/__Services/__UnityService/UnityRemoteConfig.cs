using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Course.Services.Unity
{
    public static class UnityRemoteConfig
    {
        /// <summary>
        /// Fired once a fresh config payload has been pulled down.
        /// </summary>
        public static event Action<ConfigResponse> FetchCompleted;

        /// <summary>
        /// Has InitializeAsync() completed (and auth/sign-in succeeded)?
        /// </summary>
        public static bool IsReady { get; private set; }

        /// <summary>
        /// Call this once at startup (e.g. in your Bootstrap) before any FetchConfigs().
        /// </summary>
        public static async Task InitializeAsync()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            RemoteConfigService.Instance.FetchCompleted += OnRemoteFetchCompleted;

            IsReady = true;
            Debug.Log("[RC] Initialized & signed in.");
        }

        /// <summary>
        /// Kick off a fetch; when it arrives you’ll get the FetchCompleted event.
        /// </summary>
        public static void FetchConfigs()
        {
            if (!IsReady)
            {
                Debug.LogWarning("[RC] Not ready – forgot InitializeAsync?");
                return;
            }

            var userAttributes = new UserAttributes();
            var appAttributes  = new AppAttributes();

            RemoteConfigService.Instance.FetchConfigs(userAttributes, appAttributes);
            Debug.Log("[RC] Fetch requested.");
        }

        private static void OnRemoteFetchCompleted(ConfigResponse response)
        {
            Debug.Log($"[RC] Fetch complete: {response.requestOrigin}  success={response.status}");
            FetchCompleted?.Invoke(response);
        }
        
        public struct UserAttributes { }
        public struct AppAttributes  { }
    }
}