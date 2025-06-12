using Course.Attribute;
using Course.Attribute.Bullet;
using Course.Control;
using Course.Control.Player;
using Course.Control.Turret;
using Course.Core;
using Course.Effect;
using Course.ObjectPool;
using Course.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Course.Installation
{
    public class TurretInstaller : MonoBehaviour
    {
        [Header("Turret & Shooting")]
        [SerializeField] Transform turretTransform;
        [SerializeField] Transform bulletSpawnPoint;
        [SerializeField] Bullet bulletPrefab;

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
        
        private Transform _anchor;
        private IHealthBehaviour _healthBehaviour;
        private IJumpBehaviour _jumpBehaviour;

        public void Initialize(IHealthBehaviour healthBehaviour, IJumpBehaviour jumpBehaviour)
        {
            _healthBehaviour = healthBehaviour;
            _jumpBehaviour = jumpBehaviour;
        }
        
        public void AwakeInitialize() 
        {
            _anchor = new GameObject("Bullet Anchor").transform;
        }

        public void StartInitialize()
        {
            _rotator = new Rotator();
            _bulletPool = new BulletPool(bulletPrefab, _anchor);
            _bulletFactory = new BulletFactory(_bulletPool);
            _shooter = new Shooter(_bulletFactory);
            new TurretBehaviour.TurretBuilder(turretBehaviour)
                .WithInputReader(inputReader)
                .WithRotator(_rotator)
                .WithShooter(_shooter)
                .WithHealthBehaviour(_healthBehaviour)
                .WithSpawnTransform(bulletSpawnPoint)
                .WithTurretTransform(turretTransform)
                .WithJumpBehaviour(_jumpBehaviour)
                .Build();
        }

        private void Awake()
        {
            AwakeInitialize();
        }

        private void Start()
        {
            StartInitialize();
        }
    }
}

