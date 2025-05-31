using System;
using UnityEngine;

namespace Course.Attribute
{
    public interface IAttributeBehaviour
    {
        event Action OnValueChanged;
        float MaxValue { get; }

        float Value { get; }

        float Fraction { get; }

        void ChangeValue(float value);

        void ResetValue();
    }    
}

