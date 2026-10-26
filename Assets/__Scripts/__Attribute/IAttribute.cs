using System;

namespace Course.Attribute
{
    public interface IAttribute
    {
        float Value { get; }
        event Action OnValueChanged;
        float MaxValue { get; }
        
        float BaseValue { get; }
        void ChangeValue(float value);
        float Fraction();
        void SetMaxValue(float value);
    }       
}

