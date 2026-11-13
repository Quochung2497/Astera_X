using System;
using UnityEngine;

namespace Course.Core
{
    public interface IOffScreenWrapper
    {
        event Action<GameObject> OnWrap;
    }
}

