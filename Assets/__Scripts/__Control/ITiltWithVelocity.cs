using UnityEngine;

namespace Course.Control
{
    public interface ITiltWithVelocity
    {
        void Tilt(Vector2 inputDir, float currentSpeed, float deltaTime);
    }
}
