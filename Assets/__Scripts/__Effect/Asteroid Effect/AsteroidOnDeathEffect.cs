using Course.Attribute;
using Course.Core;
using UnityEngine;

namespace Course.Effect
{
    public class AsteroidOnDeathEffect : OnTriggerEffect<Asteroid>
    {
        public override void Initialize(Asteroid asteroid)
        {
            if (_initialized) return;

            base.Initialize(asteroid);
            _initialized = true;

            if (isActiveAndEnabled)
                _owner.OnDie += HandleOnTriggerEffect;
        }

        protected override void OnEnable()
        {
            if (_initialized)
                _owner.OnDie += HandleOnTriggerEffect;
        }

        protected override void OnDisable()
        {
            if (_initialized)
                _owner.OnDie -= HandleOnTriggerEffect;
        }

        protected override float GetScaleFactor()
        {
            float scaleFactor = (float)_owner.size / AsteraXManager.TryGetInstance()
                .asteroidsSO.GetGlobalMaxParentSize();
            return Mathf.Clamp01(scaleFactor);
        }
    }
}
