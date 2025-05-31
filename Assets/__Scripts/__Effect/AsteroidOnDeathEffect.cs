using Course.Attribute;
using Course.Core;
using UnityEngine;

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
            _owner.OnDie += HandleOnDeath;
            _initialized = true;
        }

        protected override void OnEnable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnDie += HandleOnDeath;
        }


        private void OnDisable()
        {
            if(!_initialized || _owner == null)
                return;
            _owner.OnDie -= HandleOnDeath;
        }
        
        protected override void HandleOnDeath()
        {
            if (particlesEffectContainer == null || _asteroid == null)
                return;

            float scaleFactor = GetScaleFactor();
            particlesEffectContainer?.TriggerEffect(transform.position, Mathf.Clamp01(scaleFactor));
        }

        protected override float GetScaleFactor()
        {
            return (float)_asteroid.size / AsteraXManager.TryGetInstance().asteroidsSO.initialSize; // Current size divide initial size eg: size 3 / 3 = 1, size 2/3 = 0.66, size 1 /3 = 0.33
        }
    }
}
