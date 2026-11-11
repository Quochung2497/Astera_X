using System;
using Course.Attribute;

namespace Course.Control.Asteroid
{
    public class AsteroidHealth : IAsteroidHealth
    {
        private readonly Func<int, IDamageable> _createHealth;
        private IDamageable _health;
        private int _pointValue;

        public AsteroidHealth(Func<int, IDamageable> createHealth)
        {
            _createHealth = createHealth;
        }

        public void ResetHealth(int hp)
        {
            int maxHealth = Math.Max(1, hp);
            if (_health == null)
                _health = _createHealth(maxHealth);
            else
                _health.SetMaxValue(maxHealth);
        }

        public void SetPointValue(int points)
        {
            _pointValue = Math.Max(0, points);
        }

        public (bool Accepted, bool Died, int? AwardedPoints) HitByBullet(float damage)
        {
            if (_health.IsDead || damage <= 0) return default;

            _health.ChangeValue(-damage);
            bool died = _health.IsDead;
            return (true, died, died ? _pointValue : (int?)null);
        }

        public (bool Accepted, bool Died, int? AwardedPoints) HitByPlayer()
        {
            if (_health.IsDead) return default;

            _health.ChangeValue(-_health.Value);
            return (true, _health.IsDead, null);
        }
    }
}
