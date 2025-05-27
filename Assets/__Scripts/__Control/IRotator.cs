using UnityEngine;

namespace Course.Control
{
    public interface IRotator
    {
        void Aim(Transform target, Vector2 screenPos, Camera cam);
    }
}

