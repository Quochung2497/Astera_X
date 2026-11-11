using UnityEngine;

namespace Course.Effect
{
    public abstract class OnTriggerEffect<T> : MonoBehaviour, ITriggerEffect<T> where T : class
    {
        [Header("Particle Effect")]
        [SerializeField] protected ParticlesEffectProvider particlesEffectContainer;

        protected T _owner;

        protected bool _initialized = false;

        /// <summary>
        /// Assigns the owner that supplies the trigger for this effect.
        /// </summary>
        public virtual void Initialize(T owner)
        {
            if (_initialized) return;

            _owner = owner;
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        /// <summary>
        /// Plays the configured effect using the scale supplied by the subclass.
        /// </summary>
        protected virtual void HandleOnTriggerEffect()
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

