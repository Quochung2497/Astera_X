using System;
using UnityEngine;

namespace Course.Core
{
    public interface IOffScreenWrapper
    {
        void Wrap(ScreenBounds bounds);
        event Action<GameObject> OnWrap;
    }
}

