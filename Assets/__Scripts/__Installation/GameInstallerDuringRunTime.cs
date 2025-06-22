using Course.Attribute;
using Course.Control;
using Course.Core;
using Course.Level;
using Course.Services.Achievements;
using Course.Services.SaveService;
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
        
        [SerializeField] private SaveManager saveManager;

        [SerializeField] private CustomizationManager customizationManager;
        
        [Provide]
        public IScoreBehaviour ProvideScore() => score;
        [Provide]
        public IHealthBehaviour ProvideHealth() => health;

        [Provide]
        public ILevel ProvideLevel() => levelBehaviour;

        [Provide]
        public IAsteraX ProvideAsteraX() => manager;
        
        [Provide]
        public IAchievementManager ProvideAchievementManager() => achievementManager;
        
        [Provide]
        public ISaveManager ProvideSaveManager() => saveManager;
        
        [Provide] 
        public ICustomization ProvideCustomizationManager() => customizationManager;
    }
}
