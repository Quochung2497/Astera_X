using Course.Attribute;
using Course.Control;
using Course.Core;
using Course.Level;
using Course.ScriptableObject;
using Course.Services;
using Course.Services.Achievements;
using Course.Services.SaveService;
using Course.Services.Shop;
using Course.Utility;
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
        private PlayerShipConfig playerShipConfig;

        [SerializeField] private LevelBehaviour levelBehaviour;
        [SerializeField] private AsteraXManager manager;

        [SerializeField] private AchievementManager achievementManager;
        
        [SerializeField] private SaveManager saveManager;

        [SerializeField] private CustomizationManager customizationManager;
        
        [SerializeField] private PurseBehaviour purseBehaviour;
        
        [SerializeField] private ShopManager shopManager;

        private InputResolver _inputResolver;
        private IDamageable _playerDamageable;
        private IHealthBehaviour _playerHealth;

        private void Awake()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer || !Application.isMobilePlatform) return;

            int mobileQuality = System.Array.IndexOf(QualitySettings.names, "Medium");
            if (mobileQuality >= 0 && QualitySettings.GetQualityLevel() != mobileQuality)
                QualitySettings.SetQualityLevel(mobileQuality);
        }

        [Provide]
        public IInputResolver ProvideInputResolver() => GetInputResolver();

        [Provide]
        public IInputReader ProvideInputReader() => GetInputResolver();

        private InputResolver GetInputResolver() => _inputResolver ??= new InputResolver();
        
        [Provide]
        public IScoreBehaviour ProvideScore() => score;

        [Provide]
        private PlayerShipConfig ProvidePlayerShipConfig() => playerShipConfig;

        [Provide]
        private IDamageable ProvidePlayerDamageable() =>
            _playerDamageable ??= new Damageable(playerShipConfig.JumpCount);

        [Provide]
        public IHealthBehaviour ProvideHealth() =>
            _playerHealth ??= new PlayerHealth(ProvidePlayerDamageable());

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
