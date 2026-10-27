using Course.Attribute;
using Course.Core;
using UnityEngine;

namespace Course.Effect
{
    public class AsteroidOnDeathEffect : OnTriggerEffect<IHealthBehaviour>
    {
        // Private Fields
        private IAsteroid _asteroid;

        // Public
        public void Initialize(IHealthBehaviour health, IAsteroid asteroid)
        {
            if (_initialized) return;

            _asteroid ??= asteroid;
            base.Initialize(health);
            _initialized = true;

            if (isActiveAndEnabled)
                _owner.OnDie += HandleOnTriggerEffect;
        }

        protected override void OnEnable()
        {
            if (!_initialized || _owner == null)
                return;
            _owner.OnDie += HandleOnTriggerEffect;
        }


        protected override void OnDisable()
        {
            if (!_initialized || _owner == null)
                return;
            _owner.OnDie -= HandleOnTriggerEffect;
        }

        protected override void HandleOnTriggerEffect()
        {
            if (particlesEffectContainer == null || _asteroid == null
                || (_asteroid is Object unityObject && unityObject == null))
                return;

            float scaleFactor = GetScaleFactor();
            particlesEffectContainer?.TriggerEffect(transform.position, Mathf.Clamp01(scaleFactor));
        }

        protected override float GetScaleFactor()
        {
            return (float)_asteroid.size / AsteraXManager.TryGetInstance()
                .asteroidsSO.GetGlobalMaxParentSize();
        }
    }
}
