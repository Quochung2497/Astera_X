using System;
using Course.Attribute;
using Course.Effect;
using UnityEngine;

namespace Course.Effect
{
    [RequireComponent(typeof(IHealthBehaviour))]
    public abstract class OnTriggerEffect<T> : MonoBehaviour, ITriggerEffect<T> where T : class
    {
        [Header("Which particle‐effect provider to call when this object dies")]
        [SerializeField] protected ParticlesEffectProvider particlesEffectContainer;

        protected T _owner;

        protected bool _initialized = false;

        /// <summary>
        /// Call this exactly once (after Awake but before any death can occur),
        /// passing in the IHealthBehaviour that owns this object.
        /// E.g. in Asteroid.Setup(), you will do:
        ///     _deathEffect.Initialize(_health, this);
        /// </summary>
        public virtual void Initialize(T owner)
        {
            if (_initialized) return;

            _owner = owner;
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        /// <summary>
        /// This is called whenever the IHealthBehaviour fires its OnDie event.
        /// We compute a “scaleFactor” (via the subclass override) and call the provider.
        /// </summary>
        protected virtual void HandleOnDeath()
        {
            if (particlesEffectContainer == null || _owner == null)
                return;     // no provider assigned → do nothing

            // Position is simply the current GameObject’s world position
            Vector3 position = transform.position;

            // Let the subclass decide “how big” this effect should be
            float scaleFactor = GetScaleFactor();

            // Forward to the provider (which in turn calls VFXManager)
            particlesEffectContainer.TriggerEffect(position, scaleFactor);
        }

        /// <summary>
        /// Subclasses override this to tell “how big” the effect is. 
        /// E.g. an asteroid might do (size / initialSize), a bullet might always return 1f, etc.
        /// </summary>
        protected abstract float GetScaleFactor();
    }  
}

