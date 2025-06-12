using Course.Attribute;
using Course.Core;
using Course.Level;
using Course.Services.Achievements;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Installation
{
    public class GameInstallerDuringRunTime : MonoBehaviour, IDependencyProvider
    {
        [SerializeField]
        private ScoreBehaviour score;
        [SerializeField]
        private HealthBehaviour health;

        [SerializeField] private LevelBehaviour levelBehaviour;
        [SerializeField] private AsteraXManager manager;

        [SerializeField] private AchievementManager achievementManager;
        
        [Provide]
        public IScoreBehaviour ProvideScore() => score;
        [Provide]
        public IHealthBehaviour ProvideHealth() => health;

        [Provide]
        public ILevel ProvideLevel() => levelBehaviour;

        [Provide]
        public IAsteraX ProvideAsteraX() => manager;
        
        [Provide]
        public AchievementManager ProvideAchievementManager() => achievementManager;
    }
}
