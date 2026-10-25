using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UnityConsent;

namespace Course.Services.Unity
{
    public static class UnityAnalytics
    {
        private const int PendingEventLimit = 64;
        private static readonly Queue<(string Name, CustomEvent Data)> PendingEvents = new();

        public static bool IsReady { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            IsReady = false;
            PendingEvents.Clear();
        }

        public static async Task InitializeAsync()
        {
            var options = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
            GiveConsent();
            IsReady = true;

            while (PendingEvents.Count > 0)
            {
                var pending = PendingEvents.Dequeue();
                RecordNow(pending.Name, pending.Data);
            }
        }

        public static void GiveConsent()
        {
            // Preserve the demo's existing automatic opt-in using Unity 6's consent API.
            var consent = EndUserConsent.GetConsentState();
            consent.AnalyticsIntent = ConsentStatus.Granted;
            EndUserConsent.SetConsentState(consent);
            Debug.Log("[Analytics] Data collection enabled for the demo.");
        }

        public static void LogLevelStart(int level)
        {
            Record("level_start", new Dictionary<string, object> {
                { "level", level }
            });
        }

        public static void LogAchievementUnlocked(string achievementId)
        {
            Record("achievement_unlocked", new Dictionary<string, object> {
                { "achievement_id", achievementId }
            });
        }

        public static void LogGameOver(int finalScore, int finalLevel, bool gotHighScore)
        {
            Record("game_over", new Dictionary<string, object> {
                { "score", finalScore },
                { "level", finalLevel },
                { "gotHighScore", gotHighScore }
            });
        }

        public static void LogShipPartChoice(int bodyIndex, int turretIndex)
        {
            Record("ShipPartChoice", new Dictionary<string, object> {
                { "Body", bodyIndex },
                { "Turret", turretIndex }
            });
        }

        private static void Record(string eventName, Dictionary<string, object> parameters)
        {
            var data = new CustomEvent(eventName);
            foreach (var parameter in parameters)
                data.Add(parameter.Key, parameter.Value);

            if (!IsReady)
            {
                if (PendingEvents.Count == PendingEventLimit)
                {
                    PendingEvents.Dequeue();
                    Debug.LogWarning("[Analytics] Startup buffer full; dropping the oldest pending event.");
                }

                PendingEvents.Enqueue((eventName, data));
                return;
            }

            RecordNow(eventName, data);
        }

        private static void RecordNow(string eventName, CustomEvent data)
        {
            AnalyticsService.Instance.RecordEvent(data);
            AnalyticsService.Instance.Flush();
            Debug.Log($"[Analytics] Recorded '{eventName}'; upload requested.");
        }
    }
}
