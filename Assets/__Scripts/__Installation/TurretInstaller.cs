using System.Linq;
using Course.Attribute;
using Course.Attribute.Bullet;
using Course.Control;
using Course.Control.Player;
using Course.Control.Turret;
using Course.Core;
using Course.Effect;
using Course.ObjectPool;
using Course.ScriptableObject;
using Course.Utility;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Installation
{
    public class TurretInstaller : MonoBehaviour
    {
        [Header("Turret & Shooting")]
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] private BulletConfig  defaultBulletConfig;
        [SerializeField]
        private StatsProvider statsProvider = null;

        [Header("Trail VFX")]
        [Tooltip("Assign the ExhaustTrailWrapBehaviour prefab here.")]
        [SerializeField] private ExhaustTrailWrapBehaviour trailPrefab;
        
        [Header("Input")]
        [SerializeField] InputReader inputReader;
        
        [Header("Turret")]
        [SerializeField]
        TurretBehaviour turretBehaviour;
        
        private Rotator _rotator;
        private Shooter _shooter;
        
        private IFactory<Bullet> _bulletFactory;
        private BulletPool _bulletPool;
        
        private IHealthBehaviour _healthBehaviour;
        private IJumpBehaviour _jumpBehaviour;
        private TurretBehaviour.TurretBuilder _builder;
        
        [Inject]
        private CustomizationManager _customizationManager;

        public void Initialize(IHealthBehaviour healthBehaviour, IJumpBehaviour jumpBehaviour)
        {
            _healthBehaviour = healthBehaviour;
            _jumpBehaviour = jumpBehaviour;
        }

        public void AwakeInitialize(){}

        public void StartInitialize()
        {
            _rotator = new Rotator();
            _bulletPool = new BulletPool(bulletPrefab, new GameObject("BulletAnchor").transform);
            _bulletFactory = new BulletFactory(_bulletPool);
            statsProvider.RegisterStat(Stats.Speed,  defaultBulletConfig.speed);
            statsProvider.RegisterStat(Stats.Health, defaultBulletConfig.lifetime);
            statsProvider.RegisterStat(Stats.Damage, defaultBulletConfig.damage);
            _shooter = new Shooter(_bulletFactory, statsProvider.Get(Stats.Speed).MaxValue, statsProvider.Get(Stats.Health).MaxValue, statsProvider.Get(Stats.Damage).MaxValue);
            _builder = new TurretBehaviour.TurretBuilder(turretBehaviour)
                .WithInputReader(inputReader)
                .WithRotator(_rotator)
                .WithHealthBehaviour(_healthBehaviour)
                .WithJumpBehaviour(_jumpBehaviour);
            _customizationManager.OnApplyShipPart += HandleShipPartApplied;
        }

        private void Awake()
        {
            AwakeInitialize();
        }

        private void Start()
        {
            StartInitialize();
        }
        
        private void OnDestroy()
        {
            _customizationManager.OnApplyShipPart -= HandleShipPartApplied;
        }
        private void HandleShipPartApplied(PartType part, Transform mount, ShipPartSO.StatModifier[] mods)
        {
            if (part != PartType.Turret) return;

            foreach (var mod in mods)
                statsProvider.SetMaxValue(mod.stats, mod.percentBonus);
            
            RebuildShooter();
            
            // re-initialize the behaviour
            RebuildWith(mount, mount);
        }
        
        private void RebuildShooter()
        {
            float speed    = statsProvider.Get(Stats.Speed).MaxValue;
            float lifetime = statsProvider.Get(Stats.Health).MaxValue;
            float damage   = statsProvider.Get(Stats.Damage).MaxValue;
            _shooter.Reconfigure(speed, lifetime, damage);
        }

        private void RebuildWith(Transform t, Transform s)
        {
            _builder
                .WithShooter(_shooter)
                .WithTurretTransform(t)
                .WithSpawnTransform(s)
                .Build();
        }
    }
}

