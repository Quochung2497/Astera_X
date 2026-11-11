using System;

namespace Course.Attribute
{
    public class PlayerHealth : IHealthBehaviour
    {
        private readonly IDamageable _damageable;

        public PlayerHealth(IDamageable damageable)
        {
            _damageable = damageable;
        }

        public event Action OnValueChanged;
        public event Action OnDie;

        public float MaxValue => _damageable.MaxValue;
        public float Value => _damageable.Value;
        public float Fraction => _damageable.Fraction();
        public bool IsDead => _damageable.IsDead;

        public void ChangeValue(float value)
        {
            if (_damageable.IsDead) return;

            _damageable.ChangeValue(value);
            if (IsDead)
                OnDie?.Invoke();
            OnValueChanged?.Invoke();
        }
    }
}
