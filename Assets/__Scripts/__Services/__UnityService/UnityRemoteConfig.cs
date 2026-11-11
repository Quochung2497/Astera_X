using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Course.Services.Unity
{
    public static class UnityRemoteConfig
    {
        private static Task<(bool Succeeded, string Json)> _pendingFetch;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => _pendingFetch = null;

        public static Task<(bool Succeeded, string Json)> FetchSpawnRulesAsync()
        {
            if (_pendingFetch is { IsCompleted: false })
                return _pendingFetch;

            return _pendingFetch = FetchAsync();
        }

        private static async Task<(bool Succeeded, string Json)> FetchAsync()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var service = RemoteConfigService.Instance;
            Debug.Log("[RC] Fetch requested.");
            var config = await service.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
            var status = service.requestStatus;
            Debug.Log($"[RC] Fetch complete: {config.origin} status={status}");

            bool succeeded = status == ConfigRequestStatus.Success && config.origin == ConfigOrigin.Remote;
            return (succeeded, succeeded ? config.GetJson("levelSpawnRules", string.Empty) : null);
        }

        public struct UserAttributes { }
        public struct AppAttributes { }
    }
}
