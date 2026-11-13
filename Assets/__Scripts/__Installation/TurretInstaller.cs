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


        [Header("Trail VFX")]
        [Tooltip("Assign the ExhaustTrailWrapBehaviour prefab here.")]
        [SerializeField] private ExhaustTrailWrapBehaviour trailPrefab;
        
        
        [Header("Turret")]
        [SerializeField]
        TurretBehaviour turretBehaviour;
        
        private readonly IStatsProvider _stats = new StatsProvider();

        private Rotator _rotator;
        private Shooter _shooter;
        
        private IFactory<Bullet> _bulletFactory;
        private BulletPool _bulletPool;
        
        private IPlayerActionState _actions;
        private TurretBehaviour.TurretBuilder _builder;
        
        [Inject] private IInputReader _inputReader;

        [Inject]
        private ICustomization _customizationManager;

        public void Initialize(IPlayerActionState actions) => _actions = actions;

        public void AwakeInitialize(){}

        public void StartInitialize()
        {
            _rotator = new Rotator();
            _bulletPool = new BulletPool(bulletPrefab, new GameObject("BulletAnchor").transform);
            _bulletFactory = new BulletFactory(_bulletPool);
            _stats.RegisterStat(Stats.Speed,  defaultBulletConfig.speed);
            _stats.RegisterStat(Stats.Health, defaultBulletConfig.lifetime);
            _stats.RegisterStat(Stats.Damage, defaultBulletConfig.damage);
            _shooter = new Shooter(_bulletFactory, _stats.Get(Stats.Speed).MaxValue, _stats.Get(Stats.Health).MaxValue, _stats.Get(Stats.Damage).MaxValue);
            _builder = new TurretBehaviour.TurretBuilder(turretBehaviour)
                .WithInputReader(_inputReader)
                .WithRotator(_rotator)
                .WithActionState(_actions);
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

            _stats.ApplyBonus(Stats.Speed, ShipPartSO.StatModifier.GetBonusFor(mods, Stats.Speed));
            _stats.ApplyBonus(Stats.Health, ShipPartSO.StatModifier.GetBonusFor(mods, Stats.Health));
            _stats.ApplyBonus(Stats.Damage, ShipPartSO.StatModifier.GetBonusFor(mods, Stats.Damage));
            
            RebuildShooter();
            
            // re-initialize the behaviour
            RebuildWith(mount, mount);
        }
        
        private void RebuildShooter()
        {
            float speed    = _stats.Get(Stats.Speed).MaxValue;
            float lifetime = _stats.Get(Stats.Health).MaxValue;
            float damage   = _stats.Get(Stats.Damage).MaxValue;
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

