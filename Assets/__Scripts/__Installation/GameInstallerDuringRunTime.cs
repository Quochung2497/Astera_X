using Course.Attribute;
using Course.Control;
using Course.Core;
using Course.Level;
using Course.Services;
using Course.Services.Achievements;
using Course.Services.SaveService;
using Course.Services.Shop;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Installation
{
    [DefaultExecutionOrder(-100)]
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
        
        [SerializeField] private PurseBehaviour purseBehaviour;
        
        [SerializeField] private ShopManager shopManager;
        
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
        [Provide]
        public IPurse ProvidePurseBehaviour() => purseBehaviour;
        [Provide]
        public IShopService ProvideShopManager() => shopManager;
    }
}
