using UnityEngine;

namespace Course.Core
{
    public interface IOffScreenWrapper
    {
        void Wrap(ScreenBounds bounds);
        // void Wrap(Vector2 screenMin, Vector2 screenMax);
    }
}

