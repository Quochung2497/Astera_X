using Course.Control.Asteroid;
using Course.Core;
using Course.Effect;
using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Course.Attribute
{
    [RequireComponent(typeof(Rigidbody), typeof(OffScreenWrapper))]
    public class Asteroid : MonoBehaviour, IPoolObject<Asteroid>, IAsteroid
    {
        public int          size { get; private set; }
        
        #region Private Fields

        private Rigidbody           _rb; 
        private OffScreenWrapper    _wrapper;
        private IObjectPool<Asteroid> _pool;
        private IAsteraX _asteraXManager;
        private IAsteroidBehaviour _asteroidBehaviour;
        private Collider _collider; // Used to prevent multiple collisions in a single frame
        private IHealthBehaviour _health; // Used to handle damage logic if needed
        private IDamageable _damageable;
        private AsteroidOnDeathEffect _asteroidOnDeathEffect;
        private const string BulletTag = "Bullet";
        private const string PlayerTag = "Player";

        #endregion
    
        #region IPoolObject API
        /// <summary>
        /// Sets the object pool managing this asteroid instance.
        /// </summary>
        /// <param name="pool">The object pool to associate with this asteroid.</param>
        public void SetPool(IObjectPool<Asteroid> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Resets the asteroid's state.
        /// Enables the wrapper, sets the Rigidbody to non-kinematic, enables the collider, 
        /// and resets the physics velocities to zero.
        /// </summary>
        public void Reset()
        {
            _wrapper.enabled = true;
            _rb.isKinematic = false;

            _collider.enabled = true;
            // Reset physics velocities
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Releases the asteroid back to the object pool.
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }
        
        #endregion

        #region IAsteroid Implementation

        public void Initialize(OffScreenWrapper wrapper, IHealthBehaviour health, AsteroidOnDeathEffect asteroidOnDeathEffect)
        {
            _rb = GetComponent<Rigidbody>();
            _wrapper = wrapper;
            _asteraXManager = AsteraXManager.TryGetInstance();
            _collider = GetComponent<Collider>();
            _health = health;
            _asteroidOnDeathEffect = asteroidOnDeathEffect;
            _asteroidBehaviour = new AsteroidBehaviour(
                _asteraXManager, 
                frag => frag.Setup(), 
                pos  => ScreenBounds.TryGetInstance().OOB(pos),
                (self, coll) => self.OnHitInternal(coll)
            );
        }
        
        /// <summary>
        /// Initializes the asteroid by enabling necessary components, snapping its position to the z=0 plane,
        /// and setting its initial velocity.
        /// </summary>
        public void InitAsteroid()
        {
            _wrapper.enabled = true;
            _rb.isKinematic = false;
            _collider.enabled = true;
            // Snap this GameObject to the z=0 plane
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
            // Initialize the velocity for this Asteroid
            InitVelocity();
        }

        /// <summary>
        /// Sets the size of the asteroid.
        /// </summary>
        /// <param name="amount">The size value to set.</param>
        public void SetSize(int amount)
        {
            size = amount;
        }

        /// <summary>
        /// Initializes a cluster of asteroids by releasing existing child asteroids,
        /// setting the size of the cluster, and spawning child asteroids recursively.
        /// </summary>
        /// <param name="clusterSize">The size of the asteroid cluster.</param>
        public void InitializeCluster(int clusterSize)
        {
            foreach (var old in GetComponentsInChildren<Asteroid>(includeInactive: true))
                if (old != this)
                    old.Release();
    
            SetSize(clusterSize);
            Setup();  
            SpawnChildrenRecursively(clusterSize);
        }
        
        #endregion

        #region Private methods

        /// <summary>
        /// Recursively spawns child asteroids based on the specified size level.
        /// Delegates the spawning logic to the asteroid behavior.
        /// </summary>
        /// <param name="sizeLevel">The size level of the child asteroids to spawn.</param>
        private void SpawnChildrenRecursively(int sizeLevel)
        {
            _asteroidBehaviour.SpawnChildren(sizeLevel, transform, gameObject.name);
        }

        /// <summary>
        /// Initializes the asteroid as a child asteroid.
        /// Disables the collider and wrapper, sets the Rigidbody to kinematic,
        /// and adjusts the local scale relative to the parent's lossy scale.
        /// </summary>
        private void InitAsteroidChild()
        {
            _collider.enabled = false; 
            _wrapper.enabled = false;
            _rb.isKinematic = true;
            transform.localScale = transform.localScale.ComponentDivide(transform.parent.lossyScale);
        }

        /// <summary>
        /// Initializes the velocity of the asteroid.
        /// Calculates the initial linear and angular velocity using the asteroid behavior.
        /// </summary>
        private void InitVelocity()
        {
            var info = _asteroidBehaviour.CalculateInitialVelocity(transform.position, size);

            _rb.linearVelocity = info.Linear;
            _rb.angularVelocity = info.Angular;
        }

        /// <summary>
        /// Sets up the asteroid's properties such as scale and initialization state.
        /// Determines whether the asteroid is a child or parent and initializes accordingly.
        /// </summary>
        private void Setup()
        {
            transform.localScale = Vector3.one * size * _asteraXManager.asteroidsSO.asteroidScale;
            int hp = _asteraXManager.asteroidsSO.healthForAsteroidSize[size];
            _damageable = new Damageable(hp);
            _health?.Initialize(_damageable);
            _asteroidOnDeathEffect?.Initialize(_health,this);
            if (parentIsAsteroid)
            {
                InitAsteroidChild();
            }
            else
            {
                InitAsteroid();
            }
        }

        /// <summary>
        /// Handles internal logic when the asteroid is hit.
        /// Currently, this method is empty and serves as a placeholder for future implementation.
        /// </summary>
        /// <param name="coll">The collision data.</param>
        private void OnHitInternal(Collision coll) {}

        /// <summary>
        /// Determines whether the asteroid's parent is another asteroid.
        /// </summary>
        private bool parentIsAsteroid => transform.parent?.TryGetComponent<Asteroid>(out _) ?? false;


        #endregion

        #region Unity API
        
        /// <summary>
        /// Unity's OnEnable method, called when the object becomes enabled and active.
        /// Adds the asteroid to the manager's tracking list.
        /// </summary>
        private void OnEnable()
        {
            _asteraXManager.AddAsteroid(this);
            _health.OnDie += Release;
        }

        /// <summary>
        /// Unity's OnDisable method, called when the object becomes disabled or inactive.
        /// Resets the asteroid and removes it from the manager's tracking list.
        /// </summary>
        private void OnDisable()
        {
            Reset();
            _asteraXManager.RemoveAsteroid(this);
            _health.OnDie -= Release;
        }

        /// <summary>
        /// Unity's OnCollisionEnter method, called when the collider enters a collision.
        /// Handles collision logic, including bubbling up to the parent asteroid if applicable,
        /// and processing hits from bullets or the player.
        /// </summary>
        /// <param name="coll">The collision data.</param>
        private void OnCollisionEnter(Collision coll)
        {
            // Bubble up to parent if this is a child asteroid
            if (transform.parent?.TryGetComponent<Asteroid>(out var parent) ?? false)
            {
                parent.OnCollisionEnter(coll);
                return;
            }

            var other = coll.gameObject;
            if (other.CompareTag(BulletTag) || coll.transform.root.CompareTag(PlayerTag))
            {
                _asteroidBehaviour.OnHit(this, coll);
                if (other.TryGetComponent<Bullet.Bullet>(out var bullet))
                {
                    EventBus<AddScore>.Raise(new AddScore(_asteraXManager.asteroidsSO.pointsForAsteroidSize[size]));
                    _health.ChangeValue(bullet.Damage);                    
                }
            }
        }

        #endregion
    }
}
