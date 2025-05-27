using Course.Attribute.Bullet;
using Course.Control;
using Course.Control.Turret;
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

        [Header("Input")]
        [SerializeField] InputReader inputReader;
        
        [Header("Turret")]
        [SerializeField]
        TurretBehaviour turretBehaviour;

        private Rotator _rotator;
        private Shooter _shooter;
        private BulletFactory _bulletFactory;
        private BulletPool _bulletPool;
        private Transform _anchor;
        
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
            turretBehaviour.Initialize(inputReader,
                _rotator,
                _shooter,
                turretTransform,
                bulletSpawnPoint);
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

