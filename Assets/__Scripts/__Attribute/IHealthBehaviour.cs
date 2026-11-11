using System;

namespace Course.Attribute
{
    public interface IHealthBehaviour : IAttributeBehaviour
    {
        event Action OnDie;
        bool IsDead { get; }
    }
}
