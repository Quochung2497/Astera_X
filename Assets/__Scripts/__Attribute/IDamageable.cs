using System;
using UnityEngine;

namespace Course.Attribute
{
    public interface IDamageable : IAttribute
    {
        bool IsDead { get; }
    }    
}

