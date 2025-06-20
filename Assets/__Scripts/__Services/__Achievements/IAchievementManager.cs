using System;
using Course.ScriptableObject;

namespace Course.Services.Achievements
{
    public interface IAchievementManager
    {
        event Action<string, float> OnHighScore;
        event Action<AchievementSO> OnShowAchievement;
        event Action<AchievementSO> OnUnlocked; 
        void Acknowledge();
        float HighScore { get; }
        void SetHighScore(int candidate);
        bool DidHitNewHighThisSession { get; }
    }
}