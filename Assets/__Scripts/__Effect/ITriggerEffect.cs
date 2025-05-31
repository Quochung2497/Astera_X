using UnityEngine;

namespace Course.Effect
{
    /// <summary>
    /// Any component that wants to react to a “death” event on some owner type T
    /// should implement this.  For example, if T is IHealthBehaviour, 
    /// Initialize(T) will subscribe to that health’s OnDie event.
    /// Later on, you could use T = IShieldBehaviour or whatever.
    /// </summary>
    public interface ITriggerEffect<T> where T : class
    {
        /// <summary>
        /// Must be called exactly once (after Awake, before the object can die).
        /// Typically you pass in the T object whose “OnDie” or similar event you want to subscribe to.
        /// </summary>
        void Initialize(T owner);
    }
}

