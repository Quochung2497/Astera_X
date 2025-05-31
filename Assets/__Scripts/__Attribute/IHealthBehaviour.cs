using System;
using UnityEngine;

namespace Course.Attribute
{
    public interface IHealthBehaviour : IAttributeBehaviour
    {
        void Initialize(IDamageable damageable);
        event Action OnDie;
        bool IsDead { get; }
    }
}
