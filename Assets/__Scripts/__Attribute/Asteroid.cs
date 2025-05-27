using Course.Control.Asteroid;
using Course.Core;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Course.Attribute
{
    [RequireComponent(typeof(Rigidbody), typeof(OffScreenWrapper))]
    public class Asteroid : MonoBehaviour, IPoolObject<Asteroid>, IAsteroid
    {
        public int          size { get; private set; }
        
        private Rigidbody           _rb; 
        private OffScreenWrapper    _wrapper;
        private IObjectPool<Asteroid> _pool;
        private IAsteraX _asteraXManager;
        private IAsteroidBehaviour _asteroidBehaviour;
        private bool _immune = false; // Used to prevent multiple collisions in a single frame
    
        #region IPoolObject API
        public void SetPool(IObjectPool<Asteroid> pool)
        {
            _pool = pool;
        }
        
        public void Reset()
        {
            _wrapper.enabled    = true;
            _rb.isKinematic     = false;
            
            _immune              = false;
            // reset physics velocities
            _rb.linearVelocity        = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        
        public void Release()
        {
            _pool.Release(this);
        }
        
        #endregion
    
        public void InitAsteroid()
        {
            _wrapper.enabled = true;
            _rb.isKinematic = false;
            _immune = false;
            // Snap this GameObject to the z=0 plane
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
            // Initialize the velocity for this Asteroid
            InitVelocity();
        }
    
        public void SetSize(int amount)
        {
            size = amount;
        }
        
        public void InitializeCluster(int clusterSize)
        {
            foreach (var old in GetComponentsInChildren<Asteroid>(includeInactive: true))
                if (old != this)
                    old.Release();
            
            SetSize(clusterSize);
            Setup();  
            SpawnChildrenRecursively(clusterSize);
        }
        
        private void SpawnChildrenRecursively(int sizeLevel)
        {
            _asteroidBehaviour.SpawnChildren(sizeLevel, transform, gameObject.name);
        }
        
        private void InitAsteroidChild()
        {
            _immune = true; 
            _wrapper.enabled = false;
            _rb.isKinematic = true;
            transform.localScale = transform.localScale.ComponentDivide(transform.parent.lossyScale);
        }
    
        private void InitVelocity()
        {
            var info = _asteroidBehaviour.CalculateInitialVelocity(transform.position, size);

            _rb.linearVelocity        = info.Linear;
            _rb.angularVelocity = info.Angular;
        }
        
        private void Setup()
        {
            transform.localScale = Vector3.one * size * _asteraXManager.asteroidsSO.asteroidScale;
            if (parentIsAsteroid)
            {
                InitAsteroidChild();
            }
            else
            {
                InitAsteroid();
            }
        }
            
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _wrapper = GetComponent<OffScreenWrapper>();
            _asteraXManager = AsteraXManager.TryGetInstance();
            _asteroidBehaviour = new AsteroidBehaviour(
                _asteraXManager, 
                frag => frag.Setup(), 
                pos  => ScreenBounds.TryGetInstance().OOB(pos),
                (self, coll) => self.OnHitInternal(coll),
                self => self.Release()
                );
        }

        private void OnEnable()
        {
            _asteraXManager.AddAsteroid(this);
        }
    
        private void OnDisable()
        {
            Reset();
            _asteraXManager.RemoveAsteroid(this);
        }
        
        private void OnHitInternal(Collision coll){}
        
        private bool parentIsAsteroid => transform.parent?.TryGetComponent<Asteroid>(out _) ?? false;
    
        private void OnCollisionEnter(Collision coll)
        {
            if (_immune) return;

            // bubble up to parent if this is a child asteroid
            if (transform.parent?.TryGetComponent<Asteroid>(out var parent) ?? false)
            {
                parent.OnCollisionEnter(coll);
                return;
            }

            var other = coll.gameObject;
            if (other.CompareTag("Bullet") || coll.transform.root.CompareTag("Player"))
            {
                if (other.CompareTag("Bullet"))
                    Destroy(other);
                
                _asteroidBehaviour.OnHit(this, coll);
            }
        }
    }
}
