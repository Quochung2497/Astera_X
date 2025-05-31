using System;
using UnityEngine;

namespace Course.Attribute
{
    public class Score : IAttribute
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Score"/> class with the specified maximum value.
        /// Sets the current value to the maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value of the score.</param>
        public Score(float maxValue)
        {
            MaxValue = maxValue;
            Value = maxValue;
        }
        #endregion


        #region IAttribute<float> Implementation

        
        /// <summary>
        /// Event triggered when the value of the score changes.
        /// </summary>
        public event Action OnValueChanged;
        
        /// <summary>
        /// Gets the current value of the score.
        /// </summary>
        public float Value { get; private set; }
        
        
        /// <summary>
        /// Gets the maximum value of the score.
        /// </summary>
        public float MaxValue { get; private set; }
        
        /// <summary>
        /// Calculates the fraction of the current value relative to the maximum value.
        /// </summary>
        /// <returns>
        /// A float representing the fraction of the current value to the maximum value.
        /// Returns 0 if the maximum value is negative.
        /// </returns>
        public float Fraction()
        {
            if(MaxValue > 0)
                return Value / MaxValue;
            if (MaxValue == 0)
                return Value / 1;
            return 0;
        }
        
        
        /// <summary>
        /// Changes the current value of the score by the specified amount.
        /// Triggers the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="value">The amount to change the current value by.</param>
        public void ChangeValue(float value)
        {
            Value += value;
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Resets the current value of the score to the maximum value.
        /// </summary>
        public void ResetValue()
        {
            ChangeValue(MaxValue);
        }

        #endregion
    }
}
