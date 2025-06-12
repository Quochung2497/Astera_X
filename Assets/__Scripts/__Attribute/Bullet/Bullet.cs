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
        private Vector3       _launchPosition;
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
        /// Launches the bullet with the specified position and applies velocity.
        /// </summary>
        /// <param name="launchPosition">The position to launch the bullet from.</param>
        public void Launch(Vector3 launchPosition)
        {
            _launchPosition = launchPosition;
            if(_launchPosition != null)
                _rb.linearVelocity = _launchPosition * _speed;
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
            _rb.linearVelocity = Vector2.zero;
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
            _wrapper = GetComponent<IOffScreenWrapper>();
        }

        /// <summary>
        /// Resets the bullet and starts the self-destruction coroutine when enabled.
        /// </summary>
        private void OnEnable()
        {
            _spawnGuid = Guid.NewGuid();
            _alreadyReleased   = false;
            _wrapper.OnWrap += HandleWrap;
            Reset();
            _lifetimeCoroutine = StartCoroutine(SelfDestruct());
        }

        /// <summary>
        /// Resets the bullet and stops all coroutines when disabled.
        /// </summary>
        private void OnDisable()
        {
            _wrapper.OnWrap -= HandleWrap;
            Reset();
            StopAllCoroutines();
        }
        
        private void HandleWrap(GameObject go)
        {
            if (go != gameObject) return;
            EventBus<BulletWrapped>.Raise(new BulletWrapped(_spawnGuid));
        }

        /// <summary>
        /// Handles collision events. Releases the bullet if it collides with an object layer as "Asteroid".
        /// Add score if the collided object is an asteroid.
        /// Score are added based on the asteroid's size.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer != LayerNameProvider.GetLayer(LayerName.Asteroid))
                return;
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }
            Release();
            EventBus<AsteroidShot>.Raise(new AsteroidShot());
            EventBus<AsteroidHitByBullet>
                .Raise(new AsteroidHitByBullet(_spawnGuid));
        }
        

        #endregion
    }
}

