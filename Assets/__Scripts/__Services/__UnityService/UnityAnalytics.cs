using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

namespace Course.Services.Unity
{
    public static class UnityAnalytics
    {
        public static bool IsReady { get; private set; }

        public static async Task InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            GiveConsent();
            IsReady = true;
        }
        
        public static void GiveConsent()
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log($"Consent has been provided. The SDK is now collecting data!");
        }

        public static void LogLevelStart(int level)
        {
            if (!IsReady) return;
            AnalyticsEvent.LevelStart(
                "LevelStart",
                new Dictionary<string, object> {
                    { "level",   level },
                    { "time",    DateTime.UtcNow.ToString("o") }
                }
            );
            Debug.Log($"[Analytics] Sent LevelStart event for level {level}");
            AnalyticsService.Instance.Flush();
        }
        
        public static void LogAchievementUnlocked(string achievementId)
        {

            if (!IsReady)
                return;

            AnalyticsEvent.AchievementUnlocked(
                achievementId,
                new Dictionary<string, object> {
                    { "time", DateTime.UtcNow.ToString("o") }
                }
            );

            Debug.Log($"[Analytics] Sent AchievementUnlocked for '{achievementId}'");
            AnalyticsService.Instance.Flush();
        }
        
        public static void LogGameOver(int finalScore, int finalLevel, bool gotHighScore)
        {
            if (!IsReady)
                return;

            AnalyticsEvent.GameOver(
                "GameOver",                         
                new Dictionary<string, object> {   
                    { "time",          DateTime.UtcNow.ToString("o") },
                    { "score",         finalScore },
                    { "level",         finalLevel },
                    { "gotHighScore",  gotHighScore }
                }
            );

            Debug.Log(
                $"[Analytics] Sent GameOver – score:{finalScore}, " +
                $"level:{finalLevel}, gotHighScore:{gotHighScore}"
            );
            AnalyticsService.Instance.Flush();
        }
        
        public static void LogShipPartChoice(Dictionary<string, object> partSelectionCounts)
        {
            if (!IsReady) return;

            // Add or update timestamp
            partSelectionCounts["time"] = DateTime.UtcNow.ToString("o");

            AnalyticsEvent.Custom("ShipPartChoice", partSelectionCounts);

            // Debug output
            var entries = string.Join(", ",
                partSelectionCounts.Select(kv => $"{kv.Key}={kv.Value}")
            );
            Debug.Log($"[Analytics] Sent ShipPartChoice: {entries}");

            AnalyticsService.Instance.Flush();
        }
    }
}
