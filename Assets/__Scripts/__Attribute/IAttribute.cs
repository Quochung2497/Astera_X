using System;
using UnityEngine;

namespace Course.Attribute
{
    public interface IAttribute
    {
        float Value { get; }
        event Action OnValueChanged;
        float MaxValue { get; }
        void ChangeValue(float value);
        float Fraction();
        void ResetValue();
    }       
}

