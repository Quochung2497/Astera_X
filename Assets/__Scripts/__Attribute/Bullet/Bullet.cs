using System;
using System.Collections;
using Course.Core;
using Course.Utility;
using Course.Utility.Events;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Attribute.Bullet
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(OffScreenWrapper))]
    public class Bullet : MonoBehaviour,IPoolObject<Bullet>
    {
        // Cache
        private Rigidbody        _rb;
        private IObjectPool<Bullet> _pool;
        private Collider _collider;
        private bool _launched;
        private float _speed;
        private float _lifetime;
        private Guid _spawnGuid;
        private Coroutine _lifetimeCoroutine;
        private IOffScreenWrapper _wrapper;
        private bool _alreadyReleased = false;

        #region Public API

        public float Damage { get; private set; }
        
        public void Initialize(float speed, float lifetime, float damage)
        {
            _speed = speed;
            _lifetime = lifetime;
            Damage = damage;
        }
        
        /// <summary>
        /// Starts movement and lifetime after Initialize has supplied this shot's settings.
        /// </summary>
        /// <param name="launchDirection">The direction to fire.</param>
        public void Launch(Vector3 launchDirection)
        {
            _spawnGuid = Guid.NewGuid();
            _launched = true;
            _rb.linearVelocity = launchDirection * _speed;
            _collider.enabled = true;
            _lifetimeCoroutine = StartCoroutine(SelfDestruct());
        }

        #endregion

        #region IPoolObject Implementation

        /// <summary>
        /// Sets the object pool managing this bullet instance.
        /// </summary>
        /// <param name="pool">The object pool to associate with this bullet.</param>
        public void SetPool(IObjectPool<Bullet> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Resets the bullet's velocity to zero.
        /// </summary>
        public void Reset()
        {
            _launched = false;
            _collider.enabled = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }
        }

        /// <summary>
        /// Releases the bullet back to the pool or destroys it if no pool is available.
        /// </summary>
        public void Release()
        { 
            if (_alreadyReleased) return;
            _alreadyReleased = true;
            _pool.Release(this);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Coroutine that waits for the bullet's lifetime to expire before releasing it.
        /// </summary>
        /// <returns>An enumerator for the coroutine.</returns>
        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(_lifetime);
            Release();
        }

        #endregion

        #region Unity API


        /// <summary>
        /// Initializes the Rigidbody component reference.
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _wrapper = GetComponent<IOffScreenWrapper>();
            Reset();
        }

        /// <summary>
        /// Reserves a bullet; Launch starts its lifetime once configured.
        /// </summary>
        private void OnEnable()
        {
            _alreadyReleased = false;
            _wrapper.OnWrap += HandleWrap;
        }

        /// <summary>
        /// Resets the bullet and stops all coroutines when disabled.
        /// </summary>
        private void OnDisable()
        {
            _wrapper.OnWrap -= HandleWrap;
            Reset();
        }
        
        private void HandleWrap(GameObject go)
        {
            if (go != gameObject || !_launched) return;
            EventBus<BulletWrapped>.Raise(new BulletWrapped(_spawnGuid));
        }

        /// <summary>
        /// A bullet owns its hit transaction so callback order cannot apply damage twice.
        /// </summary>
        private void OnCollisionEnter(Collision other)
        {
            if (!_launched || other.gameObject.layer != LayerNameProvider.GetLayer(LayerName.Asteroid))
                return;

            var asteroid = other.gameObject.GetComponent<IAsteroid>();
            if (asteroid == null) return;

            try
            {
                if (asteroid.TryHitByBullet(Damage))
                {
                    EventBus<AsteroidShot>.Raise(new AsteroidShot());
                    EventBus<AsteroidHitByBullet>.Raise(new AsteroidHitByBullet(_spawnGuid));
                }
            }
            finally
            {
                Release();
            }
        }

        #endregion
    }
}

