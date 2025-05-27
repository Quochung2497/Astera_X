using System.Collections;
using Course.Core;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Attribute.Bullet
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(OffScreenWrapper))]
    public class Bullet : MonoBehaviour,IPoolObject<Bullet>
    {
        [Header("Bullet Settings")]
        [SerializeField] float lifetime = 2f;
        [SerializeField] float speed = 20f;

        // Cache
        private Rigidbody        _rb;
        private IObjectPool<Bullet> _pool;
        private Vector3       _launchPosition;

        public void Launch(Vector3 launchPosition)
        {
            _launchPosition = launchPosition;
            if(_launchPosition != null)
                _rb.linearVelocity = _launchPosition * speed;
        }
        
        public void SetPool(IObjectPool<Bullet> pool)
        {
            _pool = pool;
        }

        public void Reset()
        { 
            _rb.linearVelocity = Vector2.zero;
        }

        public void Release()
        {
            if (_pool != null)
                _pool.Release(this);
            else
                Destroy(gameObject);
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Reset();
            StartCoroutine(SelfDestruct());
        }

        private void OnDisable()
        {
            Reset();
            StopAllCoroutines();
        }

        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(lifetime);
            Release();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Asteroid"))
                return;
            Release();
        }
    }
}

