using System;
using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Achievements/ConditionSO Base")]
    public abstract class ConditionSO : UnityEngine.ScriptableObject
    {
        /// <summary>
        /// Fired when the condition is satisfied.
        /// </summary>
        public event Action OnConditionMet;

        /// <summary>
        /// Subscribe to EventBus or other triggers.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Unsubscribe on teardown.
        /// </summary>
        public abstract void Uninitialize();

        /// <summary>
        /// Call to mark this condition satisfied.
        /// </summary>
        protected void RaiseMet() => OnConditionMet?.Invoke();
    }
}

