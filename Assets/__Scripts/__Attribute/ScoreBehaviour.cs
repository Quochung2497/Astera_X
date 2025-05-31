using System;
using Course.Utility.Events;
using UnityEngine;

namespace Course.Attribute
{
    public class ScoreBehaviour : MonoBehaviour, IScoreBehaviour
    {

        #region Private Fields

        /// <summary>
        /// Represents the score attribute for the behavior.
        /// </summary>
        private IAttribute _score;

        /// <summary>
        /// Represents the binding for the AddScore event.
        /// </summary>
        private EventBinding<AddScore> _addScoreBinding;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the score attribute if it has not been set.
        /// </summary>
        /// <param name="score">The score attribute to initialize.</param>
        public void Initialize(IAttribute score)
        {
            _score ??= score;
        }

        #endregion
        
        #region IScoreBehaviour Implementation

        /// <summary>
        /// Event triggered when the score value changes.
        /// </summary>
        public event Action OnValueChanged;

        /// <summary>
        /// Gets the maximum value of the score.
        /// </summary>
        public float MaxValue => _score.MaxValue;

        /// <summary>
        /// Gets the current value of the score.
        /// </summary>
        public float Value => _score.Value;

        /// <summary>
        /// Gets the fraction of the current score value relative to the maximum value.
        /// </summary>
        public float Fraction => _score.Fraction();

        /// <summary>
        /// Changes the score value and triggers the OnValueChanged event.
        /// </summary>
        /// <param name="value">The amount to change the score by.</param>
        public void ChangeValue(float value)
        {
            _score.ChangeValue(value);
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Resets the score value to its initial state.
        /// </summary>
        public void ResetValue()
        {
            _score.ResetValue();
        }

        #endregion

        #region Unity Life

        /// <summary>
        /// Unity's OnEnable method, called when the object becomes enabled.
        /// Registers the AddScore event binding to the event bus.
        /// </summary>
        private void OnEnable()
        {
            _addScoreBinding = new EventBinding<AddScore>(e => ChangeValue(e.Amount));
            EventBus<AddScore>.Register(_addScoreBinding);
        }

        /// <summary>
        /// Unity's OnDisable method, called when the object becomes disabled.
        /// Deregisters the AddScore event binding from the event bus.
        /// </summary>
        private void OnDisable()
        {
            EventBus<AddScore>.Deregister(_addScoreBinding);
        }

        #endregion
    }    
}

