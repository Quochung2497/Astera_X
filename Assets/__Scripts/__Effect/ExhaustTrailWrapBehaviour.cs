using System;
using Course.Core;
using UnityEngine;

namespace Course.Effect
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ExhaustTrailWrapBehaviour : MonoBehaviour
    {
        [Header("Exhaust Trail Settings")] 
        [SerializeField]
        private float framesToSkip = 1f;
        
        private Transform        _targetTransform;
        private Rigidbody        _targetBody;
        private ParticleSystem   _ps;
        private ParticleSystem.EmissionModule _emitter;
        private IOffScreenWrapper _offScreenWrapper;
        private readonly ExhaustTrailEmission _emission = new ExhaustTrailEmission();
        private bool _isInitialize = false;

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

            _targetTransform = targetTransform ?? throw new ArgumentNullException(nameof(targetTransform));
            _targetBody = _targetTransform.GetComponent<Rigidbody>();

            _offScreenWrapper.OnWrap += HandleWrap;
            _isInitialize = true;
        }

        private void LateUpdate()
        {
            if (!_isInitialize || _targetTransform == null)
                return;
            
            if (_emission.ConsumeSuppressedFrame())
            {
                _emitter.enabled = false;
                return;
            }
            
            _emitter.enabled = ExhaustTrailEmission.ShouldEmit(_targetBody.linearVelocity.sqrMagnitude);
        }

        private void HandleWrap(GameObject obj)
        {
            if (obj != _targetTransform.gameObject) return;
            _emitter.enabled = false;
            _ps.Clear(true);
            _emission.SuppressFor(framesToSkip);
        }

        private void OnEnable()
        {
            if (!_isInitialize) return;
            _emission.SuppressFor(0f);
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
