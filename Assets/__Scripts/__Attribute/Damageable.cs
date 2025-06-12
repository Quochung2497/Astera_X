using System;
using UnityEngine;

namespace Course.Attribute
{
    public class Damageable : IDamageable
    {
        /// <summary>
        /// Indicates whether the jump attribute is in a "dead" state.
        /// </summary>
        private bool _isDead = false;

        /// <summary>
        /// Initializes a new instance of the Jump class with the specified maximum value.
        /// Sets the initial value to the maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value for the jump attribute.</param>
        public Damageable(float maxValue)
        {
            MaxValue = maxValue;
            Value = maxValue;
        }

        #region IDamageable Implementation

        /// <summary>
        /// Event triggered when the value of the jump attribute changes.
        /// </summary>
        public event Action OnValueChanged;

        /// <summary>
        /// Gets the current value of the jump attribute.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Gets the maximum value of the jump attribute.
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// Changes the value of the jump attribute by the specified amount.
        /// Triggers the OnValueChanged event and updates the "dead" state if the value drops to zero or below.
        /// </summary>
        /// <param name="change">The amount to change the value by.</param>
        public void ChangeValue(float change)
        {
            Value += change;
            if(Value > MaxValue && MaxValue != 0)
            {
                Value = MaxValue;
            }
            else if (Value < 0 && MaxValue != 0)
            {
                Value = 0;
            }
            OnValueChanged?.Invoke();
            if (Value <= 0 && !_isDead)
            {
                _isDead = true;
            }
        }

        /// <summary>
        /// Calculates the fraction of the current value relative to the maximum value.
        /// </summary>
        /// <returns>The fraction of the current value.</returns>
        public float Fraction()
        {
            return Value / MaxValue;
        }

        /// <summary>
        /// Resets the value of the jump attribute to its maximum value.
        /// Resets the "dead" state and triggers the OnValueChanged event.
        /// </summary>
        public void ResetValue()
        {
            Value = MaxValue;
            _isDead = false;
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Gets a value indicating whether the jump attribute is in a "dead" state.
        /// </summary>
        public bool IsDead => _isDead;

        #endregion
    }
}
