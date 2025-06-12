using System;
using System.Collections;
using Course.Core;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Course.Effect
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ExhaustTrailWrapBehaviour : MonoBehaviour
    {
        [Header("Exhaust Trail Settings")] 
        [SerializeField]
        private float framesToSkip = 1f;
        
        private Transform        _targetTransform;
        private ParticleSystem   _ps;
        private ParticleSystem.EmissionModule _emitter;
        private IObjectPool<ExhaustTrailWrapBehaviour> _pool;
        private IOffScreenWrapper _offScreenWrapper;
        private float _frameCounter = 0f;
        private bool _isInitialize = false;

        // If (velocity.sqrMagnitude > this), we emit; otherwise we don’t.
        private const float SPEED_THRESHOLD_SQR = 0.0001f;

        /// <summary>
        /// Call this once, immediately after the parent ship/bullet is built or pulled from pool.
        /// The child GameObject (‘ExhaustTrail’) should already be enabled in the prefab.
        /// </summary>
        public void Initialize(
            Transform        targetTransform,
            IOffScreenWrapper offScreenWrapper
        )
        {
            _ps = GetComponent<ParticleSystem>();
            _emitter = _ps.emission;
            _emitter.enabled = false;
            _offScreenWrapper = offScreenWrapper ?? throw new ArgumentNullException(nameof(offScreenWrapper));
            _ps.Play();

            _targetTransform = targetTransform  ?? throw new System.ArgumentNullException(nameof(targetTransform));

            _offScreenWrapper.OnWrap += HandleWrap;
            _isInitialize = true;
        }

        private void LateUpdate()
        {
            if (!_isInitialize || _targetTransform == null)
                return;
            
            if (_frameCounter > 0)
            {
                _frameCounter--;
                _emitter.enabled = false;
                return;
            }
            
            float speedSqr = _targetTransform.GetComponent<Rigidbody>().linearVelocity.sqrMagnitude;
            _emitter.enabled = speedSqr > SPEED_THRESHOLD_SQR;
        }

        private void HandleWrap(GameObject obj)
        {
            if (obj != _targetTransform.gameObject) return;
            _emitter.enabled = false;
            _ps.Clear(true);
            _frameCounter = framesToSkip;
        }

        private void OnEnable()
        {
            if (!_isInitialize) return;
            _frameCounter = 0f;
            _emitter.enabled = false;
            _offScreenWrapper.OnWrap += HandleWrap;
        }

        private void OnDisable()
        {
            if (!_isInitialize) return;
            _offScreenWrapper.OnWrap -= HandleWrap;
        }
    }
}
