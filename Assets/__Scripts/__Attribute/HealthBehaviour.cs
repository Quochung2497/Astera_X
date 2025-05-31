using System;
using UnityEngine;

namespace Course.Attribute
{
    public class HealthBehaviour : MonoBehaviour, IHealthBehaviour
    {
        /// <summary>
        /// Represents the jump attribute used in the behavior.
        /// </summary>
        private IDamageable _damageable;

        #region Constructor

        /// <summary>
        /// Initializes the jump attribute if it has not been set.
        /// </summary>
        /// <param name="damageable">The jump attribute to initialize.</param>
        public void Initialize(IDamageable damageable)
        {
            _damageable ??= damageable;
        }

        #endregion

        #region IJumpBehaviour Implementation

        /// <summary>
        /// Event triggered when the value of the jump attribute changes.
        /// </summary>
        public event Action OnValueChanged;

        /// <summary>
        /// Event triggered when the jump attribute enters a "dead" state.
        /// </summary>
        public event Action OnDie;

        /// <summary>
        /// Gets the maximum value of the jump attribute.
        /// </summary>
        public float MaxValue => _damageable.MaxValue;

        /// <summary>
        /// Gets the current value of the jump attribute.
        /// </summary>
        public float Value => _damageable.Value;

        /// <summary>
        /// Calculates the fraction of the current value relative to the maximum value.
        /// </summary>
        public float Fraction => Value / MaxValue;

        /// <summary>
        /// Changes the value of the jump attribute by the specified amount.
        /// Triggers the OnValueChanged event and invokes the OnDie event if the attribute is in a "dead" state.
        /// </summary>
        /// <param name="value">The amount to change the value by.</param>
        public void ChangeValue(float value)
        {
            _damageable.ChangeValue(value);
            OnValueChanged?.Invoke();
            if (IsDead) OnDie?.Invoke();
        }

        /// <summary>
        /// Resets the value of the jump attribute to its maximum value.
        /// Triggers the OnValueChanged event.
        /// </summary>
        public void ResetValue()
        {
            _damageable.ResetValue();
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Determines whether the jump attribute is in a "dead" state.
        /// </summary>
        /// <returns>True if the jump attribute is "dead"; otherwise, false.</returns>
        public bool IsDead => _damageable.IsDead;

        #endregion
    }
}
