using System;
using System.Globalization;
using Course.Attribute;
using TMPro;
using UnityEngine;

namespace Course.UI
{
    public class AttributeUI : MonoBehaviour
    {
        #region Serialize Field

        /// <summary>
        /// Reference to the TextMeshProUGUI component used to display attribute values.
        /// </summary>
        [Header("Component")]
        [SerializeField] 
        private TextMeshProUGUI text;

        /// <summary>
        /// Indicates whether the attribute represents a score.
        /// </summary>
        [Header("Score Attribute")]
        [SerializeField] 
        private bool IsScore = false;

        /// <summary>
        /// Indicates whether the attribute represents jumps.
        /// </summary>
        [Header("Jumps Attribute")] 
        [SerializeField]
        private bool IsJump = false;

        /// <summary>
        /// Label text for the jumps attribute.
        /// </summary>
        [SerializeField] 
        private string label = "Jumps";

        #endregion

        #region Private Cache

        /// <summary>
        /// Cached reference to the attribute behavior interface.
        /// </summary>
        private IAttributeBehaviour _attribute;

        /// <summary>
        /// Flag indicating whether the UI has been initialized.
        /// </summary>
        private bool _isInitialized = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the AttributeUI with the specified attribute behavior.
        /// Subscribes to the OnValueChanged event and updates the UI.
        /// </summary>
        /// <param name="attribute">The attribute behavior to bind to the UI.</param>
        public void Initialize(IAttributeBehaviour attribute)
        {
            _attribute = attribute;
            _attribute.OnValueChanged += UpdateUI;
            UpdateUI();
            _isInitialized = true;
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Updates the UI to reflect the current value of the attribute.
        /// Formats the value based on whether it is a score, jumps, or a fraction.
        /// </summary>
        private void UpdateUI()
        {
            float rawValue = _attribute.Value;
            int displayValue = Mathf.RoundToInt(rawValue);
            displayValue = Mathf.Max(displayValue, 0);
            
            if (IsScore)
                text.text = displayValue.ToString("N0", CultureInfo.InvariantCulture);
            else if (IsJump)
                text.text = displayValue + $" " + label;
            else
                text.text = _attribute.Fraction.ToString("N0", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Unity's OnEnable method.
        /// Resubscribes to the OnValueChanged event if the UI is initialized.
        /// </summary>
        private void OnEnable()
        {
            if (!_isInitialized)
                return;
            _attribute.OnValueChanged += UpdateUI;
        }

        /// <summary>
        /// Unity's OnDisable method.
        /// Unsubscribes from the OnValueChanged event if the UI is initialized.
        /// </summary>
        private void OnDisable()
        {
            if (!_isInitialized)
                return;
            _attribute.OnValueChanged -= UpdateUI;
        }

        #endregion
    }
}
