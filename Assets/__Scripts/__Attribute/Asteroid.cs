using System;
using Course.Control.Asteroid;
using Course.Core;
using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Attribute
{
    [RequireComponent(typeof(Rigidbody), typeof(OffScreenWrapper))]
    public class Asteroid : MonoBehaviour, IPoolObject<Asteroid>, IAsteroid
    {
        public int size { get; private set; }
        public int damage { get; private set; }

        public event Action OnDie;
        
        #region Private Fields

        private Rigidbody           _rb; 
        private OffScreenWrapper    _wrapper;
        private IObjectPool<Asteroid> _pool;
        private IAsteraX _asteraXManager;
        private IAsteroidBehaviour _asteroidBehaviour;
        private Collider _collider; // Used to prevent multiple collisions in a single frame
        private IAsteroidHealth _health;
        private bool _isReleased;
        private bool _isConfigured;

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
        /// Keeps pooled or unconfigured asteroids out of physics until Setup.
        /// </summary>
        public void Reset()
        {
            _isConfigured = false;
            _wrapper.enabled = false;
            _collider.enabled = false;
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        /// <summary>
        /// Releases the asteroid back to the object pool.
        /// </summary>
        public void Release()
        {
            if (_isReleased) return;
            _isReleased = true;
            _isConfigured = false;

            _pool.Release(this);
            _asteraXManager.RemoveAsteroid(this);
        }
        
        #endregion

        #region Initialization

        public void Initialize(OffScreenWrapper wrapper, IAsteroidHealth health)
        {
            _rb = GetComponent<Rigidbody>();
            _wrapper = wrapper;
            _collider = GetComponent<Collider>();
            _asteraXManager = AsteraXManager.TryGetInstance();
            _health = health;
            _asteroidBehaviour = new AsteroidBehaviour(
                _asteraXManager, 
                (frag, injectedHp) => frag.Setup(injectedHp),
                pos => ScreenBounds.TryGetInstance().OOB(pos),
                (self, coll) => OnHitInternal(coll)
            );
            Reset();
        }

        #endregion

        #region IAsteroid Implementation

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
        /// Sets the internal “score value” that will be used when the asteroid dies.
        /// </summary>
        public void SetPointValue(int pts)
        {
            _health.SetPointValue(pts);
        }

        public void SetDamageValue(int amount)
        {
            damage = Mathf.Max(0, amount);
        }

        /// <summary>
        /// Sets up the asteroid's properties such as scale and initialization state.
        /// Determines whether the asteroid is a child or parent and initializes accordingly.
        /// </summary>
        public void Setup(int hp)
        {
            if (!Application.isPlaying) return;
            transform.localScale = Vector3.one * size * _asteraXManager.asteroidsSO.asteroidScale;
            _health.ResetHealth(hp);
            if (parentIsAsteroid)
            {
                InitAsteroidChild();
            }
            else
            {
                InitAsteroid();
            }

            _isConfigured = true;
            _asteraXManager.AddAsteroid(this);
        }

        public bool TryHitByBullet(float amount)
        {
            return _isConfigured && ApplyHitResult(_health.HitByBullet(amount));
        }

        public bool TryDestroyFromPlayerCollision()
        {
            return _isConfigured && ApplyHitResult(_health.HitByPlayer());
        }
        
        #endregion

        #region Private methods

        /// <summary>
        /// Recursively spawns child asteroids based on the specified size level.
        /// Delegates the spawning logic to the asteroid behavior.
        /// </summary>
        /// <param name="sizeLevel">The size level of the child asteroids to spawn.</param>
        // private void SpawnChildrenRecursively(int sizeLevel)
        // {
        //     _asteroidBehaviour.SpawnChildren(sizeLevel, transform, gameObject.name);
        // }
        public void SpawnDescendants(
            int childSize,
            int childCount,
            int childHealth,
            int childPoints,
            int childDamage,
            int grandchildSize,
            int grandchildCount,
            int grandchildHealth,
            int grandchildPoints,
            int grandchildDamage
        )
        {
            // Simply forward to the AsteroidBehaviour, passing our own transform and name:
            _asteroidBehaviour.SpawnDescendent(
                childSize,
                childCount,
                childHealth,
                childPoints,
                childDamage,
                grandchildSize,
                grandchildCount,
                grandchildHealth,
                grandchildPoints,
                grandchildDamage,
                transform,
                gameObject.name
            );
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
        /// Reserved hit hook; currently empty and not invoked by the behaviour.
        /// </summary>
        /// <param name="coll">The collision data.</param>
        private void OnHitInternal(Collision coll) {}

        private bool ApplyHitResult((bool Accepted, bool Died, int? AwardedPoints) result)
        {
            if (result.Died)
                HandleDeath(result.AwardedPoints);

            return result.Accepted;
        }

        private void HandleDeath(int? awardedPoints)
        {
            _collider.enabled = false;

            // Detach living fragments before deactivating their parent.
            _asteroidBehaviour.OnDeath(this);
            if (awardedPoints.HasValue)
                EventBus<AddScore>.Raise(new AddScore(awardedPoints.Value));

            OnDie?.Invoke();
            Release();
        }

        /// <summary>
        /// Determines whether the asteroid's parent is another asteroid.
        /// </summary>
        private bool parentIsAsteroid => transform.parent?.TryGetComponent<IAsteroid>(out _) ?? false;


        #endregion

        #region Unity API
        
        private void OnEnable()
        {
            _isReleased = false;
        }

        private void OnDisable()
        {
            Reset();
        }

        #endregion
    }
}
