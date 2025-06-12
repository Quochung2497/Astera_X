using Course.Attribute;
using Course.Core;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Effect
{
    public class AsteroidOnDeathEffect : OnTriggerEffect<IHealthBehaviour>
    {
        // Private Fields
        private Asteroid _asteroid;

        // Public
        public void Initialize(IHealthBehaviour health, Asteroid asteroid)
        {
            _asteroid ??= asteroid;
            base.Initialize(health);
            _owner.OnDie += HandleOnTriggerEffect;
            _initialized = true;
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
            if (particlesEffectContainer == null || _asteroid == null)
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
